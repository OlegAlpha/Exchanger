﻿using System.Text;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace ExchangeService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ExchangeController : ControllerBase
{
    private const string ApiConfigurationKey = "API_KEY";
    private const string ApiUrlKey = "API_URL";
    private const string ApiKeyHeader = "apikey";
    private readonly string _apiKey;

    private readonly string _apiUrl;
    private readonly IRatesCache _cache;
    private readonly IStoryService _storyService;

    public ExchangeController(IConfiguration configuration, IRatesCache cache, IStoryService storyService)
    {
        _apiKey = configuration[ApiConfigurationKey];
        _apiUrl = configuration[ApiUrlKey];
        _cache = cache;
        _storyService = storyService;
    }

    [HttpGet]
    [Route("exchange")]
    public async Task<string> Exchange(int userId, decimal amount, string from, string to)
    {
        string result;
        var responseBody = new Response();
        try
        {
            if (!_cache.IsCreatedExchangeRate(from, to))
            {
                var client = new RestClient($"https://api.apilayer.com/fixer/convert?to={to}&from={from}&amount={amount}");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader(ApiKeyHeader, _apiKey);
                var response = await client.ExecuteGetAsync(request);

                responseBody = JsonConvert.DeserializeObject<Response>(response.Content);
                var rate = Decimal.Parse(responseBody.Info.GetPropertyValue<string>("Rate"));
                _cache.SetExchangeRate(from, to, rate);
                ExchangeRate? exchangeRate = _cache.GetExchangeRateOrDefault(from, to);
                _storyService.StoreExchange(userId, exchangeRate);
            }
            else
            {
                ExchangeRate exchangeRate = _cache.GetExchangeRateOrDefault(from, to);
                _storyService.StoreExchange(userId, exchangeRate);
                responseBody.Result = (exchangeRate.Rate * amount).ToString();
                responseBody.Query = new
                {
                    amount,
                    from,
                    to
                };
                responseBody.Success = true;
                responseBody.Date = exchangeRate.Date?.ToString("MM/dd/yyyy");
            }
        }
        catch
        {
            responseBody.Success = false;
        }

        return JsonConvert.SerializeObject(responseBody);
    }

    [HttpGet]
    [Route("latest")]
    public async Task<string?> GetLatestRates(string? @base, string? symbols)
    {
        var toCurrencies = symbols?.Split(',').ToList();
        var currencies = new Dictionary<string, decimal>();
        toCurrencies?.ForEach(currency =>
        {
            var rate = _cache.GetExchangeRateOrDefault(@base, currency);
            if (rate is null)
            {
                return;
            }

            if (_cache.IsCreatedExchangeRate(@base, currency))
            {
                currencies[currency] = _cache.GetExchangeRateOrDefault(@base, currency).Rate;
                toCurrencies.Remove(currency);
            }
        });

        var newSymbols = String.Join(",", toCurrencies);
        Response? responseBody = null;
        if (String.IsNullOrWhiteSpace(newSymbols))
        {
            var urlBuilder = new StringBuilder($"{_apiUrl}/latest?")
                .AppendIf($"symbols={newSymbols}&", String.IsNullOrWhiteSpace(newSymbols) == false)
                .AppendIf($"base={@base}", @base is not null);

            var client = new RestClient(urlBuilder.ToString());
            var request = new RestRequest();
            request.AddHeader(ApiKeyHeader, _apiKey);
            var response = await client.ExecuteAsync(request);
            responseBody = JsonConvert.DeserializeObject<Response>(response.Content);
        }

        responseBody ??= new Response()
        {
            Base = @base,
            Date = DateTime.UtcNow.ToString("MM/dd/yyyy"),
            Success = true,
            TimeStamp = DateTime.UtcNow.Millisecond.ToString()
        };
        foreach (var kv in currencies)
        {
            responseBody.Rates[kv.Key] = kv.Value.ToString();
        }

        return JsonConvert.SerializeObject(responseBody);
    }

    [HttpGet]
    [Route("symbols")]
    public async Task<string?> GetAvailableCurrencies()
    {
        var client = new RestClient($"{_apiUrl}/symbols");
        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, _apiKey);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    [HttpGet]
    [Route("timeseries")]
    public async Task<string?> GetRatesWithin(DateTime endDate, DateTime startDate, string? @base, string? symbols)
    {
        var urlBuilder = new StringBuilder(_apiUrl)
            .Append($"/timeseries?end_date={endDate.ToString("MM/dd/yyyy")}")
            .Append($"&start_date={startDate.ToString("MM/dd/yyyy")}")
            .AppendIf($"&base={@base}", @base is not null)
            .AppendIf($"&symbols={symbols}", symbols is not null);

        var client = new RestClient(urlBuilder.ToString());

        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, "GSUSaL15VGITPxaUApRmfje7H9bII7rj");

        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    [HttpGet]
    [Route("fluctuation")]
    public async Task<string> Fluctuation(DateTime start, DateTime end, string baseCurrency, params string[] currencies)
    {
        ExchangeRate? startRate = null;
        ExchangeRate? endRate = null;
        List<string> uncahcedCurrencies = new List<string>();
        List<string> cahcedCurrencies = new List<string>();
        Response responseBody;
        try
        {
            foreach (string currency in currencies)
            {

                if (_cache.IsCreatedExchangeRate(baseCurrency, currency, start))
                {
                    startRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, start);
                }

                if (_cache.IsCreatedExchangeRate(baseCurrency, currency, end))
                {
                    endRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, end);
                }

                if (startRate is null || endRate is null)
                {
                    uncahcedCurrencies.Add(currency);
                }
            }

            if (uncahcedCurrencies.Count > 0)
            {
                string currenciesRequest = string.Join(',', uncahcedCurrencies);

                var urlBuilder = new StringBuilder($"{_apiUrl}/latest?")
             .AppendIf($"start_date={start.ToString("yyyy-MM-dd")}&", true)
             .AppendIf($"end_date={end.ToString("yyyy-MM-dd")}", true)
             .AppendIf($"base={baseCurrency}", string.IsNullOrWhiteSpace(baseCurrency))
             .AppendIf($"symbols={currenciesRequest}", string.IsNullOrWhiteSpace(currenciesRequest));

                var client = new RestClient(urlBuilder.ToString());
                var request = new RestRequest();
                request.AddHeader(ApiKeyHeader, _apiKey);
                var response = await client.ExecuteAsync(request);
                responseBody = JsonConvert.DeserializeObject<Response>(response.Content);
            }
            else
            {
                responseBody = new Response()
                {
                    Base = baseCurrency,
                    EndDate = end.ToString("yyyy-MM-dd"),
                    Fluctuation = true,
                    StartDate = start.ToString("yyyy-MM-dd"),
                    Success = true,
                };

            }

            foreach (var currency in cahcedCurrencies)
            {
                startRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, start);
                endRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, end);

                string jsonObject = string.Format(
                    "{ \"change\":\"{0}\" \r\n \"change_pct\":\"{1}\" \r\n \"end_rate\":\"{2}\" \r\n \"start_rate\":\"{3}\" \r\n}",
                    startRate.Rate / endRate.Rate, startRate.Rate / endRate.Rate, start, end);

                responseBody.Rates.Add(currency, jsonObject);
            }

        }
        catch
        {
            responseBody = new Response()
            {
                Success = false,
            };
        }


        return JsonConvert.SerializeObject(responseBody);
    }

    //[HttpGet]
    //[Route("exchangeRate")]
    //public string GetExchangeRateOrDefault(string baseCurrency, string[] currencies, DateTime? date = null)
    //{
    //    bool isHistorical = date != null;
    //    bool isSuccess = true;
    //    Stopwatch timer = new Stopwatch();
    //    ExchangeRate exchangeRate;
    //    JSONBaseComponent rates = new("rates");

    //    timer.Start();
    //    try
    //    {
    //        foreach (var currency in currencies)
    //        {
    //            exchangeRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, date);
    //            rates.AddComponent(currency, exchangeRate.Rate.ToString());
    //        }
    //    }
    //    catch
    //    {
    //        isSuccess = false;
    //    }

    //    timer.Stop();

    //    InfoComponent info = new(timer.ElapsedMilliseconds, isHistorical);
    //    ExchangeResponse response = new(date ?? DateTime.UtcNow.Date, info, isSuccess);

    //    info.AddComponent("base", baseCurrency);
    //    response.AddComponent(rates);


    //    return response.Build().ToString();
    //}

    //[HttpGet]
    //[Route("exchange")]
    //public string StoreExchange(int userId, decimal amount, string from, string to)
    //{
    //    decimal result = 0;
    //    ExchangeRate exchangeRate = new();
    //    Stopwatch timer = new();
    //    bool isSuccess = true;

    //    timer.Start();
    //    try
    //    {
    //        exchangeRate = _cache.GetExchangeRateOrDefault(from, to);
    //        result = _storyService.StoreExchange(userId, amount, exchangeRate);
    //    }
    //    catch
    //    {
    //        isSuccess = false;
    //    }

    //    timer.Stop();

    //    QueryComponent query = new(exchangeRate?.From, exchangeRate?.To);
    //    InfoComponent info = new(timer.ElapsedMilliseconds, false);
    //    ExchangeResponse response = new(DateTime.UtcNow.Date, info, isSuccess, result);

    //    response.AddComponent(query);
    //    query.AddComponent("amount", amount.ToString());
    //    info.AddComponent("rate", exchangeRate?.Rate.ToString());
    //    response.AddComponent("rate", exchangeRate?.Rate.ToString());

    //    return response.Build().ToString();
    //}
    //[HttpGet]
    //[Route("fluctuation")]
    //public string Fluctuation(DateTime start, DateTime end, string baseCurrency, params string[] currencies)
    //{
    //    bool isSuccess = true;
    //    Stopwatch timer = new();
    //    ExchangeRate startRate;
    //    ExchangeRate endRate;
    //    InfoComponent info;
    //    ExchangeResponse response;
    //    JSONBaseComponent rates = new("rates");
    //    RateComponent rateComponent;

    //    timer.Start();

    //    try
    //    {
    //        foreach (string currency in currencies)
    //        {
    //            startRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, start);
    //            endRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, end);

    //            rateComponent = new RateComponent(currency, startRate.Rate, endRate.Rate);
    //            rates.AddComponent(rateComponent);
    //        }
    //    }
    //    catch
    //    {
    //        isSuccess = false;
    //    }

    //    timer.Stop();

    //    info = new InfoComponent(timer.ElapsedMilliseconds);
    //    response = new ExchangeResponse(DateTime.UtcNow.Date, info, isSuccess);
    //    response.AddComponent(rates);
    //    info.AddComponent("fluctuation", true.ToString());
    //    info.AddComponent("base", baseCurrency);
    //    info.AddComponent("start_date", start.ToString());
    //    info.AddComponent("end_date", end.ToString());

    //    return response.Build().ToString();
    //}
    //[HttpGet]
    //[Route("exchangeStory")]
    //public string GetExchangeStory(DateTime start, DateTime end, string baseCurrency, string[] currencies)
    //{
    //    bool isSuccess = true;
    //    Stopwatch timer = new();
    //    ExchangeRate exchangeRate;
    //    JSONBaseComponent dates;
    //    JSONBaseComponent rates = new("rates");

    //    timer.Start();
    //    try
    //    {
    //        for (DateTime current = start; (current - end).Days < 0; current = current.AddDays(1))
    //        {
    //            dates = new JSONBaseComponent(current.ToString());

    //            foreach (var currency in currencies)
    //            {
    //                exchangeRate = _cache.GetExchangeRateOrDefault(baseCurrency, currency, current);
    //                dates.AddComponent(currency, exchangeRate.Rate.ToString());
    //            }

    //            rates.AddComponent(dates);
    //        }
    //    }
    //    catch
    //    {
    //        isSuccess = false;
    //    }

    //    timer.Stop();

    //    InfoComponent info = new(timer.ElapsedMilliseconds);
    //    ExchangeResponse response = new(DateTime.UtcNow.Date, info, isSuccess);

    //    info.AddComponent("base", baseCurrency);
    //    info.AddComponent("start_date", start.ToString());
    //    info.AddComponent("end_date", end.ToString());
    //    info.AddComponent("timeseries", true.ToString());
    //    response.AddComponent(rates);


    //    return response.Build().ToString();
    //}
    //public string Symbols(string[] abbreviatures)
    //{
    //    bool isSuccess = true;
    //    Stopwatch stopwatch = new();
    //    JSONBaseComponent symbols = new("symbols");
    //    ExchangeResponse response;
    //    InfoComponent info;
    //    string abbreviatureName;
    //    stopwatch.Start();

    //    try
    //    {
    //        foreach (string abbreviature in abbreviatures)
    //        {
    //            abbreviatureName = _cache.GetAbbreviatureName(abbreviature);
    //            symbols.AddComponent(abbreviature, abbreviatureName);
    //        }
    //    }
    //    catch
    //    {
    //        isSuccess=false;
    //    }

    //    stopwatch.Stop();

    //    info = new InfoComponent(stopwatch.ElapsedMilliseconds);
    //    response = new ExchangeResponse(DateTime.UtcNow.Date, info, isSuccess);
    //    response.AddComponent(symbols);

    //    return response.Build().ToString();
    //}

}

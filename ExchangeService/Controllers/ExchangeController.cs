using System.Text;
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
    private readonly ICacheService _cacheService;
    private readonly IHistoryService _storyService;

    public ExchangeController(IConfiguration configuration, ICacheService cache, IHistoryService storyService)
    {
        _apiKey = configuration[ApiConfigurationKey];
        _apiUrl = configuration[ApiUrlKey];
        _cacheService = cache;
        _storyService = storyService;
    }

    [HttpGet]
    [Route("exchange")]
    public async Task<string> Exchange(int userId, decimal amount, string from, string to)
    {
        return await _cacheService.ExchageProcess(userId, amount, from, to);
    }

    [HttpGet]
    [Route("latest")]
    public async Task<string?> GetLatestRates(string? @base, string? symbols)
    {
        var toCurrencies = symbols?.Split(',').ToList();
        var currencies = new Dictionary<string, decimal>();
        toCurrencies?.ForEach(currency =>
        {
            var rate = _cacheService.GetExchangeRateOrDefault(@base, currency);
            if (rate is null)
            {
                return;
            }

            if (_cacheService.IsCreatedExchangeRate(@base, currency))
            {
                currencies[currency] = (decimal)_cacheService.GetExchangeRateOrDefault(@base, currency).Rate;
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
        string[] currencies = symbols.Split(',');
        bool isAllCached = true; 
        for (var i = startDate; i < endDate; i = i.AddDays(1))
        {
            foreach (var currency in currencies)
            {
                if (!_cacheService.IsCreatedExchangeRate(@base, currency, i))
                {
                    isAllCached = false;
                    break;
                }
            }
        }

        if (!isAllCached)
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

        Response responseBody = new Response()
        {
             Base = @base,
             EndDate =  endDate.ToString("yyyy-MM-dd"),
             StartDate = startDate.ToString("yyyy-MM-dd"),
             TimeSeries = true,
        };


        for (var i = startDate; i < endDate; i = i.AddDays(1))
        {
            string ratesJsonObject = "{";

            foreach (var currency in currencies)
            {
                ExchangeRate rate = _cacheService.GetExchangeRateOrDefault(@base, currency, i);
                string rateJson = string.Format("\"{0}\":{1},", currency, rate.Rate.ToString());
                ratesJsonObject = string.Concat(ratesJsonObject, rate);
            }

            ratesJsonObject = ratesJsonObject.Substring(0,ratesJsonObject.Length - 1);
            ratesJsonObject = string.Concat(ratesJsonObject, "},");
            responseBody.Rates[i.ToString("yyyy-MM-dd")] = ratesJsonObject;
        }

        responseBody.Rates[endDate.ToString("yyyy-MM-dd")] = 
            responseBody.Rates[endDate.ToString("yyyy-MM-dd")]
            .Substring(0, responseBody.Rates[endDate.ToString("yyyy-MM-dd")].Length - 1);

        return JsonConvert.SerializeObject(responseBody);

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

                if (_cacheService.IsCreatedExchangeRate(baseCurrency, currency, start))
                {
                    startRate = _cacheService.GetExchangeRateOrDefault(baseCurrency, currency, start);
                }

                if (_cacheService.IsCreatedExchangeRate(baseCurrency, currency, end))
                {
                    endRate = _cacheService.GetExchangeRateOrDefault(baseCurrency, currency, end);
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
                startRate = _cacheService.GetExchangeRateOrDefault(baseCurrency, currency, start);
                endRate = _cacheService.GetExchangeRateOrDefault(baseCurrency, currency, end);

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

}

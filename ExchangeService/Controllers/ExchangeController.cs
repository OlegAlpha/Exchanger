using System.Diagnostics;
using System.Net;
using System.Text;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService;
using ExchangeService.BusinessLogic.Builders.JSON;
using ExchangeService.BusinessLogic.Builders.JSON.Components;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace ExchangerService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ExchangeController : Controller
{
    private const string ApiConfigurationKey = "API_KEY";
    private const string ApiUrlKey = "API_URL";
    private const string ApiKeyHeader = "apikey";
    private readonly string _apiKey;

    private readonly string _apiUrl;
    //private readonly CachedInformer _informer;
    //private readonly Converter _converter;

    public ExchangeController(IConfiguration configuration)
    {
        _apiKey = configuration[ApiConfigurationKey];
        _apiUrl = configuration[ApiUrlKey];
    }

    [HttpGet]
    [Route("convert")]
    public async Task<string?> GetLatestRates(string? @base, string? symbols)
    {
        var urlBuilder = new StringBuilder($"{_apiUrl}/latest?")
            .AppendIf($"symbols={symbols}", symbols is not null);
        urlBuilder.AppendIf("&", urlBuilder.ToString().Contains("symbols"))
            .AppendIf($"base={@base}", @base is not null);

        var client = new RestClient(urlBuilder.ToString());
        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, _apiKey);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    [HttpGet]
    [Route("symbols?")]
    public async Task<string> GetAvailableCurrencies()
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
    [Route("{date}")]
    public async Task<string?> GetRatesForDate(DateTime date, string? @base, string? symbols)
    {
        var urlBuilder = new StringBuilder(_apiUrl)
            .Append($"/{date.ToString("MM/dd/yyyy")}?")
            .AppendIf($"base={@base}&", @base is not null)
            .AppendIf($"symbols={symbols}", symbols is not null);
    }
    //[HttpGet]
    //[Route("exchangeRate")]
    //public string GetExchangeRate(string baseCurrency, string[] currencies, DateTime? date = null)
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
    //            exchangeRate = _informer.GetExchangeRate(baseCurrency, currency, date);
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
    //public string Exchange(int userId, decimal amount, string from, string to)
    //{
    //    decimal result = 0;
    //    ExchangeRate exchangeRate = new();
    //    Stopwatch timer = new();
    //    bool isSuccess = true;

    //    timer.Start();
    //    try
    //    {
    //        exchangeRate = _informer.GetExchangeRate(from, to);
    //        result = _converter.Exchange(userId, amount, exchangeRate);
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
    //            startRate = _informer.GetExchangeRate(baseCurrency, currency, start);
    //            endRate = _informer.GetExchangeRate(baseCurrency, currency, end);

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
    //                exchangeRate = _informer.GetExchangeRate(baseCurrency, currency, current);
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
    //            abbreviatureName = _informer.GetAbbreviatureName(abbreviature);
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

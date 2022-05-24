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
        return await _cacheService.LatestRatesProcess(@base, symbols);
    }

    [HttpGet]
    [Route("symbols")]
    public async Task<string?> GetAvailableCurrencies()
    {
        return await _cacheService.GetAvailableCurrencies();
    }

    [HttpGet]
    [Route("timeseries")]
    public async Task<string?> GetRatesWithin(DateTime endDate, DateTime startDate, string? @base, string? symbols)
    {
        return await _cacheService.RatesWithinProcess(endDate, startDate, @base, symbols);
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

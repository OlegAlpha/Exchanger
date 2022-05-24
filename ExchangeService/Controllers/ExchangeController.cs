using System.Text;
using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
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
    private readonly IRedirectRequests _redirect;

    public ExchangeController(IConfiguration configuration, IRedirectRequests redirect)
    {
        _apiKey = configuration[ApiConfigurationKey];
        _apiUrl = configuration[ApiUrlKey];
        _redirect = redirect;
    }

    [HttpGet]
    [Route("exchange")]
    public async Task<string> Exchange(int userId, decimal amount, string from, string to)
    {
        return await _redirect.ExchageProcess(userId, amount, from, to);
    }

    [HttpGet]
    [Route("latest")]
    public async Task<string?> GetLatestRates(string? @base, string? symbols)
    {
        return await _redirect.LatestRatesProcess(@base, symbols);
    }

    [HttpGet]
    [Route("symbols")]
    public async Task<string?> GetAvailableCurrencies()
    {
        return await _redirect.GetAvailableCurrencies();
    }

    [HttpGet]
    [Route("timeseries")]
    public async Task<string?> GetRatesWithin(DateTime endDate, DateTime startDate, string? @base, string? symbols)
    {
        return await _redirect.RatesWithinProcess(endDate, startDate, @base, symbols);
    }

    [HttpGet]
    [Route("fluctuation")]
    public async Task<string> Fluctuation(DateTime start, DateTime end, string baseCurrency, params string[] currencies)
    {
        return await _redirect.FluctuationProcessing(start, end, baseCurrency, currencies);
    }

}

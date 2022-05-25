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
    private readonly IRedirectService _redirect;

    public ExchangeController(IRedirectService redirect)
    {
        _redirect = redirect;
    }

    [HttpPost]
    [Route("exchange")]
    public async Task<string> PostExchange(int userId, decimal amount, string from, string to)
    {
        return await _redirect.ExchangeProcess(userId, amount, from, to);
    }

    [HttpGet]
    [Route("latest")]
    public async Task<string?> PostLatestRates(string? @base, string? symbols)
    {
        return await _redirect.LatestRatesProcess(@base, symbols);
    }

    [HttpGet]
    [Route("symbols")]
    public async Task<string?> PostAvailableCurrencies()
    {
        return await _redirect.GetAvailableCurrencies();
    }

    [HttpGet]
    [Route("timeseries")]
    public async Task<string?> PostRatesWithin(DateTime endDate, DateTime startDate, string? @base, string? symbols)
    {
        return await _redirect.RatesWithinProcess(endDate, startDate, @base, symbols);
    }

    [HttpGet]
    [Route("fluctuation")]
    public async Task<string> PostFluctuation(DateTime start, DateTime end, string? baseCurrency, string[]? currencies)
    {
        return await _redirect.FluctuationProcessing(start, end, baseCurrency, currencies);
    }

}

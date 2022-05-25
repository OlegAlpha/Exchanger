﻿using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    [Route("exchange")]
    public async Task<string> Exchange(int userId, decimal amount, string from, string to)
    {
        return await _redirect.ExchangeProcess(userId, amount, from, to);
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
    public async Task<string> Fluctuation(DateTime start, DateTime end, string? baseCurrency, string[]? currencies)
    {
        return await _redirect.FluctuationProcessing(start, end, baseCurrency, currencies);
    }

}

﻿using System.Diagnostics;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService.BusinessLogic.Builders.JSON;
using ExchangeService.BusinessLogic.Builders.JSON.Components;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.AspNetCore.Mvc;

namespace ExchangerService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ExchangeController : Controller
{
    private readonly CachedInformer _informer;
    private readonly Converter _converter;

    public ExchangeController(CachedInformer informer, Converter converter)
    {
        _informer = informer;
        _converter = converter;
    }

    [HttpGet]
    [Route("exchangeRate")]
    public string GetExchangeRate(string baseCurrency, string[] currencies, DateTime? date = null)
    {
        bool isHistorical = date != null;
        bool isSuccess = true;
        Stopwatch timer = new Stopwatch();
        ExchangeRate exchangeRate;
        JSONBaseComponent rates = new("rates");

        timer.Start();
        try
        {
            foreach (var currency in currencies)
            {
                exchangeRate = _informer.GetExchangeRate(baseCurrency, currency, date);
                rates.AddComponent(currency, exchangeRate.Rate.ToString());
            }
        }
        catch
        {
            isSuccess = false;
        }

        timer.Stop();

        InfoComponent info = new(timer.ElapsedMilliseconds, isHistorical);
        ExchangeResponse response = new(date ?? DateTime.UtcNow.Date, info, isSuccess);

        info.AddComponent("base", baseCurrency);
        response.AddComponent(rates);


        return response.Build().ToString();
    }

    [HttpGet]
    [Route("exchange")]
    public string Exchange(int userId, decimal amount, string from, string to)
    {
        decimal result = 0;
        ExchangeRate exchangeRate = new();
        Stopwatch timer = new();
        bool isSuccess = true;

        timer.Start();
        try
        {
            exchangeRate = _informer.GetExchangeRate(from, to);
            result = _converter.Exchange(userId, amount, exchangeRate);
        }
        catch
        {
            isSuccess = false;
        }

        timer.Stop();

        QueryComponent query = new(exchangeRate?.From, exchangeRate?.To);
        InfoComponent info = new(timer.ElapsedMilliseconds, false);
        ExchangeResponse response = new(DateTime.UtcNow.Date, info, isSuccess, result);

        response.AddComponent(query);
        query.AddComponent("amount", amount.ToString());
        info.AddComponent("rate", exchangeRate?.Rate.ToString());
        response.AddComponent("rate", exchangeRate?.Rate.ToString());

        return response.Build().ToString();
    }
    [HttpGet]
    [Route("fluctuation")]
    public string Fluctuation(DateTime start, DateTime end, string baseCurrency, params string[] currencies)
    {
        bool isSuccess = true;
        Stopwatch timer = new();
        ExchangeRate startRate;
        ExchangeRate endRate;
        InfoComponent info;
        ExchangeResponse response;
        JSONBaseComponent rates = new("rates");
        RateComponent rateComponent;

        timer.Start();

        try
        {
            foreach (string currency in currencies)
            {
                startRate = _informer.GetExchangeRate(baseCurrency, currency, start);
                endRate = _informer.GetExchangeRate(baseCurrency, currency, end);

                rateComponent = new RateComponent(currency, startRate.Rate, endRate.Rate);
                rates.AddComponent(rateComponent);
            }
        }
        catch
        {
            isSuccess = false;
        }

        timer.Stop();

        info = new InfoComponent(timer.ElapsedMilliseconds);
        response = new ExchangeResponse(DateTime.UtcNow.Date, info, isSuccess);
        response.AddComponent(rates);
        info.AddComponent("fluctuation", true.ToString());
        info.AddComponent("base", baseCurrency);
        info.AddComponent("start_date", start.ToString());
        info.AddComponent("end_date", end.ToString());

        return response.Build().ToString();
    }
    [HttpGet]
    [Route("exchangeStory")]
    public string GetExchangeStory(DateTime start, DateTime end, string baseCurrency, string[] currencies)
    {
        bool isSuccess = true;
        Stopwatch timer = new();
        ExchangeRate exchangeRate;
        JSONBaseComponent dates;
        JSONBaseComponent rates = new("rates");

        timer.Start();
        try
        {
            for (DateTime current = start; (current - end).Days < 0; current = current.AddDays(1))
            {
                dates = new JSONBaseComponent(current.ToString());

                foreach (var currency in currencies)
                {
                    exchangeRate = _informer.GetExchangeRate(baseCurrency, currency, current);
                    dates.AddComponent(currency, exchangeRate.Rate.ToString());
                }

                rates.AddComponent(dates);
            }
        }
        catch
        {
            isSuccess = false;
        }

        timer.Stop();

        InfoComponent info = new(timer.ElapsedMilliseconds);
        ExchangeResponse response = new(DateTime.UtcNow.Date, info, isSuccess);

        info.AddComponent("base", baseCurrency);
        info.AddComponent("start_date", start.ToString());
        info.AddComponent("end_date", end.ToString());
        info.AddComponent("timeseries", true.ToString());
        response.AddComponent(rates);


        return response.Build().ToString();
    }
    public string Symbols(string[] abbreviatures)
    {
        bool isSuccess = true;
        Stopwatch stopwatch = new();
        JSONBaseComponent symbols = new("symbols");
        ExchangeResponse response;
        InfoComponent info;
        string abbreviatureName;
        stopwatch.Start();

        try
        {
            foreach (string abbreviature in abbreviatures)
            {
                abbreviatureName = _informer.GetAbbreviatureName(abbreviature);
                symbols.AddComponent(abbreviature, abbreviatureName);
            }
        }
        catch
        {
            isSuccess=false;
        }

        stopwatch.Stop();

        info = new InfoComponent(stopwatch.ElapsedMilliseconds);
        response = new ExchangeResponse(DateTime.UtcNow.Date, info, isSuccess);
        response.AddComponent(symbols);

        return response.Build().ToString();
    }

}
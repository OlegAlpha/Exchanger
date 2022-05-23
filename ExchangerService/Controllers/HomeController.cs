﻿using DataBaseLayer.Entities;
using IntermediateLayer.Builders.JSON;
using IntermediateLayer.Builders.JSON.Components;
using IntermediateLayer.Builders.JSON.Components.BaseInterface;
using IntermediateLayer.BussinesLogic.RequestProcess;
using IntermediateLayer.Models.LocalivesAlternatives;
using IntermediateLayer.Models.StaticObjects;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Exchanger.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly CachedInformator _informator;
    private readonly Converter _converter;

    public HomeController(CachedInformator informator, Converter converter)
    {
        _informator = informator;
        _converter = converter;
    }

    [HttpGet]
    [Route("exchangeRate")]
    public string GetExchangeRate(string baseCurrency, string[] currencies, DateTime? date = null)
    {
        bool isHistorical = date != null;
        bool isSuccess = true;
        Stopwatch stopwatch = new Stopwatch();
        ExchangeRate exchangeRate;
        JSONBaseComponent rates = new("rates");

        stopwatch.Start();
        try
        {
            foreach (var currency in currencies)
            {
                exchangeRate = _informator.GetExchangeRate(baseCurrency, currency, date);
                rates.AddComponent(currency, exchangeRate.Rate.ToString());
            }
        }
        catch
        {
            isSuccess = false;
        }

        stopwatch.Stop();

        InfoComponent info = new InfoComponent(stopwatch.ElapsedMilliseconds, isHistorical);
        ExchangeResponse response = new ExchangeResponse(date ?? DateTime.UtcNow.Date, info, isSuccess);

        info.AddComponent("base", baseCurrency);
        response.AddComponent(rates);


        return response.Build().ToString();
    }

    [HttpGet]
    [Route("exchange")]
    public string Exchange(int UserId, decimal amount, string from, string to)
    {
        decimal result = 0;
        ExchangeRate exchangeRate = new ExchangeRate();
        Stopwatch stopwatch = new Stopwatch();
        bool isSuccess = true;

        stopwatch.Start();
        try
        {
            exchangeRate = _informator.GetExchangeRate(from, to);
            result = _converter.Exchange(UserId, amount, exchangeRate);
        }
        catch
        {
            isSuccess = false;
        }

        stopwatch.Stop();

        QueryComponent query = new(exchangeRate?.From, exchangeRate?.To);
        InfoComponent info = new(stopwatch.ElapsedMilliseconds, false);
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
        Stopwatch stopwatch = new();
        ExchangeRate startRate;
        ExchangeRate endRate;
        InfoComponent info;
        ExchangeResponse response;
        JSONBaseComponent rates = new("rates");
        RateComponent rateComponent;

        stopwatch.Start();

        try
        {
            foreach (string currency in currencies)
            {
                startRate = _informator.GetExchangeRate(baseCurrency, currency, start);
                endRate = _informator.GetExchangeRate(baseCurrency, currency, end);

                rateComponent = new RateComponent(currency, startRate.Rate, endRate.Rate);
                rates.AddComponent(rateComponent);
            }
        }
        catch
        {
            isSuccess = false;
        }

        stopwatch.Stop();

        info = new InfoComponent(stopwatch.ElapsedMilliseconds);
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
        Stopwatch stopwatch = new Stopwatch();
        ExchangeRate exchangeRate;
        JSONBaseComponent dates;
        JSONBaseComponent rates = new("rates");

        stopwatch.Start();
        try
        {
            for (DateTime current = start; (current - end).Days < 0; current = current.AddDays(1))
            {
                dates = new JSONBaseComponent(current.ToString());

                foreach (var currency in currencies)
                {
                    exchangeRate = _informator.GetExchangeRate(baseCurrency, currency, current);
                    dates.AddComponent(currency, exchangeRate.Rate.ToString());
                }

                rates.AddComponent(dates);
            }
        }
        catch
        {
            isSuccess = false;
        }

        stopwatch.Stop();

        InfoComponent info = new InfoComponent(stopwatch.ElapsedMilliseconds);
        ExchangeResponse response = new ExchangeResponse(DateTime.UtcNow.Date, info, isSuccess);

        info.AddComponent("base", baseCurrency);
        info.AddComponent("start_date", start.ToString());
        info.AddComponent("end_date", end.ToString());
        info.AddComponent("timeseries", true.ToString());
        response.AddComponent(rates);


        return response.Build().ToString();
    }
    public string Symbols(string[] abbriviatures)
    {
        bool isSuccess = true;
        Stopwatch stopwatch = new Stopwatch();
        JSONBaseComponent symbols = new JSONBaseComponent("symbols");
        ExchangeResponse response;
        InfoComponent info;
        string abbriviatureName;
        stopwatch.Start();

        try
        {
            foreach (string abbreviature in abbriviatures)
            {
                abbriviatureName = _informator.GetAbbriviatureName(abbreviature);
                symbols.AddComponent(abbreviature, abbriviatureName);
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
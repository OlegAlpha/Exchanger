using DataBaseLayer.Entities;
using IntermediateLayer.Builders.JSON;
using IntermediateLayer.Builders.JSON.Components;
using IntermediateLayer.BussinesLogic.RequestProcess;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Exchanger.Controllers;
public class HomeController : Controller
{
    private static readonly Informator informator = new Informator();
    private static readonly Converter converter = new Converter();
    public string GetExchangeRate(string from, string to, DateTime? date = null)
    {
        bool isHistorical = date is null;
        bool isSuccess = true;
        Stopwatch stopwatch = new Stopwatch();
        ExchangeRate exchangeRate = new ExchangeRate();


        stopwatch.Start();
        try
        {
            exchangeRate = informator.GetExchangeRate(from, to, date);

        }
        catch
        {
            isSuccess = false;
        }

        stopwatch.Stop();

        QueryComponent query = new QueryComponent(exchangeRate?.From, exchangeRate?.To);
        InfoComponent info = new InfoComponent(stopwatch.ElapsedMilliseconds, isHistorical);
        ExchangeResponse response = new ExchangeResponse(date ?? DateTime.UtcNow.Date, info, isSuccess);

        response.AddComponent(query);
        response.AddComponent("rate", exchangeRate?.Rate.ToString());

        return response.Build().ToString();
    }

    public string Exchange(int UserId, decimal amount, string from, string to)
    {
        decimal result = 0;
        ExchangeRate exchangeRate = new ExchangeRate();
        Stopwatch stopwatch = new Stopwatch();
        bool isSuccess = true;

        stopwatch.Start();
        try
        {
            exchangeRate = informator.GetExchangeRate(from, to);
            result = converter.Exchange(UserId, amount, exchangeRate);
        }
        catch
        {
            isSuccess = false;
        }

        stopwatch.Stop();

        QueryComponent query = new QueryComponent(exchangeRate?.From, exchangeRate?.To);
        InfoComponent info = new InfoComponent(stopwatch.ElapsedMilliseconds, false);
        ExchangeResponse response = new ExchangeResponse(DateTime.UtcNow.Date, info, isSuccess, result);

        response.AddComponent(query);
        query.AddComponent("amount", amount.ToString());
        info.AddComponent("rate", exchangeRate?.Rate.ToString());
        response.AddComponent("rate", exchangeRate?.Rate.ToString());

        return response.Build().ToString();
    }
    public object Fluctuation(DateTime start, DateTime end, string baseCurrency, params string[] currencies)
    {
        bool isSuccess = true;
        Stopwatch startwatch = new Stopwatch();
        ExchangeRate startRate = new ExchangeRate();
        ExchangeRate endRate = new ExchangeRate();
        decimal change;

        foreach(string currency in currencies)
        {
            startRate = informator.GetExchangeRate(baseCurrency, currency, start);
            endRate = informator.GetExchangeRate(baseCurrency, currency, end);
            change = startRate.Rate / endRate.Rate;
        }

        throw new NotImplementedException();
    }

    public string GetExchangeStory()
    {
        throw new NotImplementedException();
    }
}

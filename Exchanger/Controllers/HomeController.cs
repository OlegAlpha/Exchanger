using DataBaseLayer.Entities;
using IntermediateLayer.BussinesLogic.RequestProcess;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Exchanger.Controllers;
public class HomeController : Controller
{
    private static readonly Informator informator = new Informator();
    private static readonly Converter converter = new Converter();
    public object GetExchangeRate(string from, string to, DateTime? date = null)
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

        return new
        {
            from = exchangeRate?.From,
            to = exchangeRate?.To,
            rate = exchangeRate?.Rate,
            date = date ?? DateTime.UtcNow.Date,
            success = isSuccess,
            historical = isHistorical,
            timestamp = stopwatch.ElapsedMilliseconds,
        };
    }

    public object Exchange(int UserId, decimal amount, string from, string to)
    {
        decimal result = 0;
        ExchangeRate exchangeRate = new ExchangeRate();
        Stopwatch startwatch = new Stopwatch();
        bool isSuccess = true;

        startwatch.Start();
        try
        {
            exchangeRate = informator.GetExchangeRate(from, to);
            result = converter.Exchange(UserId, amount, exchangeRate);
        }
        catch
        {
            isSuccess = false;
        }

        startwatch.Stop();

        return new
        {
            date = DateTime.UtcNow.Date,
            historical = false,

            info = new
            {
                rate = exchangeRate?.Rate,
                timestamp = startwatch.ElapsedMilliseconds,
            },

            query = new
            {
                amount = amount,
                from = from,
                to = to,
            },

            result = result,
            success = isSuccess,
        };
    }

    public string GetExchangeStory()
    {
        throw new NotImplementedException();
    }
}

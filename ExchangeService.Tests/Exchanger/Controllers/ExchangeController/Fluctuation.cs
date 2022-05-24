using System;
using ExchangerService.DataAccessLayer;
using ExchangerService.DataAccessLayer.CRUD;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace ExchangerService.Tests.Exchanger.Controllers.ExchangeController;
public class Fluctuation
{
    private ExchangerService.Controllers.ExchangeController GetController(bool fillDb = false)
    {
        var options = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase("Test").Options;
        var context = new Context(options);
        if (fillDb)
        {
            for (var i = 1; i <= 8; ++i)
            {
                context.ExchangeRates.Add(new ExchangeRate()
                {
                    From = "UAH",
                    To = "EUR",
                    Created = DateTime.Now.AddDays(-i),
                    Rate = 1m / 35
                });
            }

            context.SaveChanges();


        }
        var operation = new BasicOperation(context);
        var informator = new Informer(operation);
        var configuration = Substitute.For<IConfiguration>();
        configuration["RateLifetimeInCache"].Returns("1800000");
        configuration["MaxCountInPeriod"].Returns("10");
        configuration["ExchangeLimitedPeriodInHours"].Returns("1");
        var controller = new ExchangerService.Controllers.ExchangeController(new CachedInformer(informator, configuration), new Converter(operation, configuration));
        return controller;
    }

    [Fact]
    public void GetFluctuation()
    {
        var controller = GetController(true);
        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "UAH";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.Fluctuation(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.True(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.info.fluctuation.ToString()));
        Assert.NotEqual("", result.rates.ToString());
    }
    [Fact]
    public void GetIncorrectFluctuation()
    {
        var controller = GetController();
        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "fse";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.Fluctuation(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.info.fluctuation.ToString()));
    }
}

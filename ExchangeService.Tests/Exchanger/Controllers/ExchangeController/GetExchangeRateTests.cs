using System;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.DataAccessLayer;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace ExchangeService.Tests.Exchanger.Controllers.ExchangeController;
public class GetExchangeRateTests
{
    private ExchangeService.Controllers.ExchangeController GetController(string dbName = "Test", bool fillDb = false)
    {
        var options = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(dbName).Options;
        var context = new Context(options);
        if (fillDb)
        {
            context.ExchangeRates.Add(new ExchangeRate()
            {
                From = "USD",
                To = "UAH",
                Created = DateTime.Now,
                Rate = 33m
            });

            context.ExchangeRates.Add(new ExchangeRate()
            {
                From = "USD",
                To = "EUR",
                Created = DateTime.Now,
                Rate = 0.9m
            });

            context.ExchangeRates.Add(new ExchangeRate()
            {
                From = "USD",
                To = "UAH",
                Created = DateTime.Now.AddDays(-1),
                Rate = 33m
            });

            context.ExchangeRates.Add(new ExchangeRate()
            {
                From = "USD",
                To = "EUR",
                Created = DateTime.Now.AddDays(-1),
                Rate = 0.9m
            });

            context.SaveChanges();
        }
        var operation = new BasicOperation(context);
        var informator = new Informer(operation);
        var configuration = Substitute.For<IConfiguration>();
        configuration["RateLifetimeInCache"].Returns("1800000");
        configuration["MaxCountInPeriod"].Returns("10");
        configuration["ExchangeLimitedPeriodInHours"].Returns("1");
        var controller = new ExchangeService.Controllers.ExchangeController(new CachedInformer(informator, configuration), new StoryService(operation, configuration));
        return controller;
    }

    [Fact]
    public void GetActualData()
    {
        var controller = GetController(fillDb: true);
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", };

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.success.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.False(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
    }
    [Fact]
    public void GetActualDatas()
    {
        var controller = GetController("2 Rates", true);
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", "EUR"};

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.success.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.False(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
        Assert.NotNull(result.rates.EUR);
    }
    [Fact]
    public void GetFailedActualDatas()
    {
        var controller = GetController();

        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea", "EUR" };

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.False(bool.Parse(result.info.historical.ToString()));
    }
    [Fact]
    public void GetFailedActualData()
    {
        var controller = GetController();
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea" };

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.False(bool.Parse(result.info.historical.ToString()));
    }
    [Fact]
    public void GetHistoricData()
    {
        var controller = GetController(fillDb: true);
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", };
        DateTime dateTime = DateTime.Today.AddDays(-1); 

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
    }
    [Fact]
    public void GetHistoricDatas()
    {
        var controller = GetController(fillDb: true);
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", "EUR" };
        DateTime dateTime = DateTime.Today.AddDays(-1);

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
        Assert.NotNull(result.rates.EUR);
    }
    [Fact]
    public void GetFailedHistoricDatas()
    {
        var controller = GetController();
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea", "EUR" };
        DateTime dateTime = DateTime.Today.AddDays(-1);

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.info.historical.ToString()));
    }
    [Fact]
    public void GetFailedHistoricData()
    {
        var controller = GetController();
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea" };
        DateTime dateTime = DateTime.Today.AddDays(-1);

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.info.historical.ToString()));
    }
}

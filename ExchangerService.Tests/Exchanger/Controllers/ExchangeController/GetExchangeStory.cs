using System;
using ExchangerService.DataAccessLayer;
using ExchangerService.DataAccessLayer.CRUD;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace ExchangerService.Tests.Exchanger.Controllers.HomeController;
public class GetExchangeStory
{
    private Exchanger.Controllers.ExchangeController GetController()
    {
        var options = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase("Test").Options;
        var context = new Context(options);
        var operation = new BasicOperation(context);
        var informator = new Informer(operation);
        var configuration = Substitute.For<IConfiguration>();
        configuration["RateLifetimeInCache"].Returns("1800000");
        configuration["MaxCountInPeriod"].Returns("10");
        configuration["ExchangeLimitedPeriodInHours"].Returns("1");
        var controller = new ExchangerService.Controllers.HomeController(new CachedInformer(informator, configuration), new Converter(operation, configuration));
        return controller;
    }

    public void GetStory()
    {
        var controller = GetController();

        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "UAH";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.GetExchangeStory(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.True(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.timeseries.ToString()));
        Assert.NotEqual("", result.rates.ToString());
    }

    public void GetFailedStory()
    {
        var controller = GetController();

        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "UAH";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.GetExchangeStory(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.timeseries.ToString()));
        Assert.Equal("", result.rates.ToString());
    }
}

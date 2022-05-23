using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;
using Exchanger.Controllers;
using IntermediateLayer;
using IntermediateLayer.BussinesLogic.RequestProcess;
using DataBaseLayer.CRUD;
using Microsoft.EntityFrameworkCore;
using DataBaseLayer;
using NSubstitute;
using Microsoft.Extensions.Configuration;

namespace Exchanger.Tests.ExchangerTests.Controllers.HomeController;
public class GetExchangeStory
{
    private Exchanger.Controllers.ExchangeController GetController()
    {
        var options = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase("Test").Options;
        var context = new Context(options);
        var operation = new BasicOperation(context);
        var informator = new Informator(operation);
        var configuration = Substitute.For<IConfiguration>();
        configuration["RateLifetimeInCache"].Returns("1800000");
        configuration["MaxCountInPeriod"].Returns("10");
        configuration["ExchangeLimitedPeriodInHours"].Returns("1");
        var controller = new Exchanger.Controllers.ExchangeController(new CachedInformator(informator, configuration), new Converter(operation, configuration));
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

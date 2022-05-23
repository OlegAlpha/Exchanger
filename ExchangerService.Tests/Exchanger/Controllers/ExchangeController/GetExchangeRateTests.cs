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
public class GetExchangeRateTests
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

    [Fact]
    public void GetActualData()
    {
        var controller = GetController();
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
        var controller = GetController();
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
        var controller = GetController();
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
        var controller = GetController();
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

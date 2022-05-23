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
public class ExchangeTests
{
    private Exchanger.Controllers.HomeController GetController()
    {
        var options = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase("Test").Options;
        var context = new Context(options);
        var operation = new BasicOperation(context);
        var informator = new Informator(operation);
        var configuration = Substitute.For<IConfiguration>();
        configuration["RateLifetimeInCache"].Returns("1800000");
        configuration["MaxCountInPeriod"].Returns("10");
        configuration["ExchangeLimitedPeriodInHours"].Returns("1");
        var controller = new Exchanger.Controllers.HomeController(new CachedInformator(informator, configuration), new Converter(operation, configuration));
        return controller;
    }
    [Fact]
    public void Exchange()
    {
        var controller = GetController();
        int userId = 1;
        decimal amount = 100;
        string from = "UAH";
        string to = "EUR";

        string resultJSON = controller.Exchange(userId, amount, from, to);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.True(bool.Parse(result.success.ToString()));
        Assert.NotEqual("", result.result.ToString());
    }
    [Fact]
    public void FailExchange()
    {
        var controller = GetController();
        int userId = 1;
        decimal amount = 100;
        string from = "UAH";
        string to = "cdsv";

        string resultJSON = controller.Exchange(userId, amount, from, to);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
    }
    [Fact]
    public void SpamExchange()
    {
        var controller = GetController();
        const int spamCount = 10;
        int userId = 1;
        decimal amount = 100;
        string from = "UAH";
        string to = "EUR";

        for (int i = 0; i < spamCount; ++i){
            controller.Exchange(userId, amount, from, to);
        }

        string resultJSON = controller.Exchange(userId, amount, from, to);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
    }
}

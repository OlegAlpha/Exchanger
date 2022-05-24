using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.Controllers;
using ExchangeService.DataAccessLayer;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace ExchangeService.Tests
{
    [TestFixture]
    public class ExchangeControllerTests
    {
        private ExchangeController GetController()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration["API_KEY"].Returns("GSUSaL15VGITPxaUApRmfje7H9bII7rj");
            configuration["API_URL"].Returns("https://api.apilayer.com/fixer");
            configuration["RateLifetimeInCache"].Returns("1800000");
            configuration["MaxCountInPeriod"].Returns("10");
            configuration["ExchangeLimitedPeriodInHours"].Returns("1");

            var cache = new RatesCache(configuration);
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);
            var operation = new BasicOperation(context);
            operation.Add(new ExchangeStory()
            {
                UserId = 1,
                Created = DateTime.UtcNow.AddMinutes(-10),
                Amount =10,
                Rate = new ExchangeRate()
                {
                    From = "EUR",
                    To = "UAH",
                    Rate = 35m
                }
            });
            var storyService = new StoryService(operation, configuration);
            return new ExchangeController(configuration, cache, storyService);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.Controllers;
using ExchangeService.DataAccessLayer;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;

namespace ExchangeService.Tests
{
    [TestFixture]
    public class ExchangeControllerTests
    {
        private ExchangeController GetController()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration["API_KEY"].Returns("cRT0hBKu4TtHVhEDiOpoV78CW8Jcgr3c");
            configuration["API_URL"].Returns("https://api.apilayer.com/fixer");
            configuration["RateLifetimeInCache"].Returns("1800000");
            configuration["MaxCountInPeriod"].Returns("10");
            configuration["ExchangeLimitedPeriodInHours"].Returns("1");

            var cache = new CacheService(configuration);
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);
            var repository = new ExchangeHistoryRepository(context);
            //operation.Add(new ExchangeHistory()
            //{
            //    UserId = 1,
            //    Created = DateTime.UtcNow.AddMinutes(-10),
            //    Amount = 10,
            //    Rate = new ExchangeRate()
            //    {
            //        From = "EUR",
            //        To = "UAH",
            //        Rate = 35m
            //    }
            //});
            var historyService = new HistoryService(repository, configuration);
            return new ExchangeController(configuration, cache, historyService);
        }

        [Test]
        public void Exchange_UncachedData_ReturnsCorrectResponse()
        {
            var controller = GetController();

            var response = controller.Exchange(1, 10, "EUR", "UAH").Result;
            var responseBody = JsonConvert.DeserializeObject<Response>(response);

            Assert.That(responseBody.Success, Is.True);
            Assert.That(responseBody.Query.GetPropertyValue<decimal>("amount"), Is.EqualTo(10).Within(0.001m));
        }
    }
}

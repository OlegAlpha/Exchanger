using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace ExchangeService.BusinessLogic.Tests
{
    [TestFixture]
    public class CacheServiceTests
    {

        [Test]
        public void IsCreatedExchangeRate_NonExistingRate_ReturnsFalse()
        {
            var configuration = GetConfiguration();
            var historyService = new HistoryServiceDummy();

            var cache = new CacheService(configuration, historyService);

            var result = cache.IsCreatedExchangeRate("UAH", "UAH");
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCreatedExchangeRate_ExistingRate_ReturnsTrue()
        {
            var configuration = GetConfiguration();
            var historyService = new HistoryServiceDummy();

            var cacheService = new CacheService(configuration, historyService);
            cacheService.SetExchangeRate("EUR", "UAH", 35);
            var result = cacheService.IsCreatedExchangeRate("EUR", "UAH");
            Assert.That(result, Is.True);
        }

        private IConfiguration GetConfiguration()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration["RateLifetimeInCache"].Returns("1800000");
            return configuration;
        }

        [Test]
        public void GetExchangeRateOrDefault_NonExistingRate_ReturnsNull()
        {
            var configuration = GetConfiguration();
            var historyService = new HistoryServiceDummy();

            var cacheService = new CacheService(configuration, historyService);
            var rate = cacheService.GetExchangeRateOrDefault("EUR", "U");

            Assert.That(rate, Is.Null);
        }

        [Test]
        public void GetExchangeRateOrDefault_ExistingRate_ReturnsRate()
        {
            var configuration = GetConfiguration();
            var historyService = new HistoryServiceDummy();

            var service = new CacheService(configuration, historyService);
            service.SetExchangeRate("EUR", "USD", 1.2m);
            var rate = service.GetExchangeRateOrDefault("EUR", "USD");

            Assert.That(rate, Is.Not.Null);
            Assert.That(rate.From, Is.EqualTo("EUR"));
            Assert.That(rate.To, Is.EqualTo("USD"));
            Assert.That(rate.Rate, Is.EqualTo(1.2m).Within(0.0001m));
        }
    }

    internal class HistoryServiceDummy : IHistoryService
    {
        public Task<bool> ExchangesCountIsValid(int userId)
        {
            return Task.FromResult(true);
        }

        public void StoreExchange(int userId, ExchangeRate rate)
        {
            
        }
    }
}

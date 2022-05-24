using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;

namespace ExchangerService.BusinessLogic.Tests
{
    [TestFixture]
    public class CachedInformerTests
    {
        private CacheService GetInformer()
        {
            string configurationData = File.ReadAllText("./appsettings.json");
            Dictionary<string, object> ungroupdepConfigurations = JsonConvert.DeserializeObject<Dictionary<string, object>>(configurationData);
            var configuration = Substitute.For<IConfiguration>();

            foreach (KeyValuePair<string, object> kv in ungroupdepConfigurations)
            {
                configuration[kv.Key].Returns(kv.Value.ToString());
            }
            var informer = new CacheService(configuration);
            return informer;
        }

        [Test]
        public void IsCreatedExchangedRate_EmptyCache_ReturnsFalse()
        {
            var informer = GetInformer();

            var result = informer.IsCreatedExchangeRate("EUR", "UAH");
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCreatedExchangedRate_NonEmptyCache_ReturnsTrue()
        {
            var informer = GetInformer();
            informer.SetExchangeRate("USD", "UAH", 33);
            var result = informer.IsCreatedExchangeRate("USD", "UAH");
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetExchangeRate_NonExistingRate_ReturnsNull()
        {
            var informer = GetInformer();
            var rate = informer.GetExchangeRateOrDefault("EUR", "USD");

            Assert.That(rate, Is.Null);
        }

        [Test]
        public void GetExchangeRate_ExistingRate_ReturnsRate()
        {
            var informer = GetInformer();
            informer.SetExchangeRate("UAH", "EUR", 1m / 35);
            var rate = informer.GetExchangeRateOrDefault("UAH", "EUR");

            Assert.That(rate, Is.Not.Null);
            Assert.That(rate.From, Is.EqualTo("UAH"));
            Assert.That(rate.To, Is.EqualTo("EUR"));
            Assert.That(rate.Rate, Is.EqualTo(1m / 35).Within(0.0001m));
        }
    }
}

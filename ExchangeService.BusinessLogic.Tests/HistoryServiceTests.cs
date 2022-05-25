using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;

namespace ExchangeService.BusinessLogic.Tests
{
    [TestFixture]
    public class HistoryServiceTests
    {
        private IHistoryService GetHistoryService()
        {
            string configurationData = File.ReadAllText("./appsettings.json");
            Dictionary<string, object> ungroupdepConfigurations = JsonConvert.DeserializeObject<Dictionary<string, object>>(configurationData);
            var configuration = Substitute.For<IConfiguration>();

            foreach (KeyValuePair<string, object> kv in ungroupdepConfigurations)
            {
                configuration[kv.Key].Returns(kv.Value.ToString());
            }

            var options = new DbContextOptionsBuilder<DataAccessLayer.Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new DataAccessLayer.Context(options);
            var repository = new ExchangeHistoryRepository(context);

            return new HistoryService(repository, configuration);
        }

        [Test]
        [TestCase(11)]
        [TestCase(13)]
        public void ExchangesCountIsValid_NonExistingUserId_ReturnsTrue(int userId)
        {
            var historyService = GetHistoryService();
            var result = historyService.ExchangesCountIsValid(userId);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void ExchangesCountIsValid_ExistingUserId_ReturnsTrue(int userId)
        {
            var historyService = GetHistoryService();
            historyService.StoreExchange(userId, new ExchangeRate()
            {
                Date = DateTime.UtcNow,
                From = "EUR",
                To = "EUR",
                Rate = (double)1m
            });
            var result = historyService.ExchangesCountIsValid(userId);
            Assert.That(result, Is.True);
        }

        [Test]
        public void StoreExchange_11Times_ThrowsException()
        {
            var service = GetHistoryService();
            TestDelegate code = () =>
            {
                for (var i = 0; i < 12; ++i)
                {
                    service.StoreExchange(3, new ExchangeRate()
                    {
                        Date = DateTime.UtcNow,
                        From = "EUR",
                        To = "EUR",
                        Rate = 1
                    });
                }
            };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.DataAccessLayer;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace ExchangerService.BusinessLogic.Tests
{
    [TestFixture]
    public class HistoryServiceTests
    {
        private IHistoryService GetHistoryService()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration["MaxCountInPeriod"].Returns("10");
            configuration["ExchangeLimitedPeriodInHours"].Returns("1");

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);
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

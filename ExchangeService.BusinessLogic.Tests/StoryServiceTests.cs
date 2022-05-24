using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.DataAccessLayer;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace ExchangerService.BusinessLogic.Tests
{
    [TestFixture]
    public class StoryServiceTests
    {
        private IStoryService GetStoryService()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration["MaxCountInPeriod"].Returns("10");
            configuration["ExchangeLimitedPeriodInHours"].Returns("1");

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);
            var operation = new BasicOperation(context);

            return new StoryService(operation, configuration);
        }

        [Test]
        [TestCase(11)]
        [TestCase(13)]
        public void ExchangesCountIsValid_NonExistingUserId_ReturnsTrue(int userId)
        {
            var storyService = GetStoryService();
            var result = storyService.ExchangesCountIsValid(userId);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void ExchangesCountIsValid_ExistingUserId_ReturnsTrue(int userId)
        {
            var storyService = GetStoryService();
            storyService.StoreExchange(userId, new ExchangeRate()
            {
                Date = DateTime.UtcNow,
                From = "EUR",
                To = "EUR",
                Rate = 1m
            });
            var result = storyService.ExchangesCountIsValid(userId);
            Assert.That(result, Is.True);
        }

        [Test]
        public void StoreExchange_11Times_ThrowsException()
        {
            var service = GetStoryService();
            TestDelegate code = () =>
            {
                for (var i = 0; i < 12; ++i)
                {
                    service.StoreExchange(3, new ExchangeRate()
                    {
                        Date = DateTime.UtcNow,
                        From = "EUR",
                        To = "EUR",
                        Rate = 1m
                    });
                }
            };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}

using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ExchangeService.DataAccessLayer.Tests
{
    [TestFixture]
    public class ExchangeRepositoryTests
    {
        private IExchangeHistoryRepository GetRepository()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);
            return new ExchangeHistoryRepository(context);
        }

        [Test]
        public void Add_ExchangeHistory_AddsToDatabase()
        {
            var repository = GetRepository();
            repository.Add(new ExchangeHistory()
            {
                Created = DateTime.UtcNow,
                Amount = 25,
                Rate = new ExchangeRate()
                {
                    Date = DateTime.UtcNow,
                    From = "EUR",
                    To = "UAH",
                    Rate = (double)35m
                },
                UserId = 1
            });

            var histories = repository.FindByUserIdOrDefault(1).Result;
            Assert.That(histories, Is.Not.Empty);
            var history = histories.First();
            Assert.That(history.Rate.From, Is.EqualTo("EUR"));
            Assert.That(history.Rate.To, Is.EqualTo("UAH"));
            Assert.That(history.Amount, Is.EqualTo(25).Within(1e-3));
        }

        [Test]
        public void FindByUserIdOrDefault_NonExistingId_ReturnsNull()
        {
            var repository = GetRepository();

            var history = repository.FindByUserIdOrDefault(10).Result.FirstOrDefault();

            Assert.That(history, Is.Null);
        }
    }
}

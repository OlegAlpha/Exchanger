using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

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
        public void Add_ExchangeStory_AddsToDatabase()
        {
            var repository = GetRepository();
            repository.Add(new ExchangeStory()
            {
                Created = DateTime.UtcNow,
                Amount = 25,
                Rate = new ExchangeRate()
                {
                    Date = DateTime.UtcNow,
                    From = "EUR",
                    To = "UAH",
                    Rate = 35m
                },
                UserId = 1
            });

            var story = repository.FindByUserIdOrDefault(1);
            Assert.That(story, Is.Not.Null);
            Assert.That(story.Rate.From, Is.EqualTo("EUR"));
            Assert.That(story.Rate.To, Is.EqualTo("UAH"));
        }

        [Test]
        public void FindByUserIdOrDefault_NonExistingId_ReturnsNull()
        {
            var repository = GetRepository();

            var story = repository.FindByUserIdOrDefault(10);

            Assert.That(story, Is.Null);
        }
    }
}

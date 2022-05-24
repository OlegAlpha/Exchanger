using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.DataAccessLayer.Tests
{
    [TestFixture]
    public class BasicOperationTests
    {
        private BasicOperation GetOperation()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);
            return new BasicOperation(context);
        }

        [Test]
        public void Add_ExchangeHistory_AddsToDatabase()
        {
            var operation = GetOperation();
            operation.Add(new ExchangeHistory()
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

            var history = operation.FindByUserIdOrDefault(1);
            Assert.That(history, Is.Not.Null);
            Assert.That(history.Rate.From, Is.EqualTo("EUR"));
            Assert.That(history.Rate.To, Is.EqualTo("UAH"));
        }

        [Test]
        public void FindByUserIdOrDefault_NonExistingId_ReturnsNull()
        {
            var operation = GetOperation();

            var history = operation.FindByUserIdOrDefault(10);

            Assert.That(history, Is.Null);
        }
    }
}

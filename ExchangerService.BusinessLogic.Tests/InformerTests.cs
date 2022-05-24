using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangerService.DataAccessLayer;
using ExchangerService.DataAccessLayer.CRUD;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.EntityFrameworkCore;

namespace ExchangerService.BusinessLogic.Tests
{
    [TestFixture]
    public class InformerTests
    {
        [Test]
        public void GetExchangeRate_NonExistingRate_Throws()
        {
            var informer = GetInformer();

            Assert.Throws<ArgumentException>(() => informer.GetExchangeRate("UAH", "EUR"));
        }

        private Informer GetInformer()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .Options;
            var context = new Context(options);

            context.ExchangeRates.Add(new ExchangeRate()
            {
                Created = DateTime.Now,
                From = "EUR",
                To = "USD",
                Rate = 1.1m
            });

            var operation = new BasicOperation(context);
            var informer = new Informer(operation);
            return informer;
        }

        [Test]
        public void GetExchangeRate_ExistingRate_ReturnsCorrectRate()
        {
            var informer = GetInformer();

        }
    }
}

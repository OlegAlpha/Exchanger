﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangerService.DataAccessLayer;
using ExchangerService.DataAccessLayer.CRUD;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace ExchangerService.Tests.Exchanger.Controllers.ExchangeController
{
    public class SymbolsTests
    {
        private ExchangerService.Controllers.ExchangeController GetController(string dbName, bool fillContext)
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(dbName)
                .Options;
            var context = new Context(options);
            if (fillContext)
            {
                context.CurrencyAssociations.Add(new CurrencyAssociation()
                {
                    Abbreviature = "UAH",
                    Name = "Ukrainian hryvnia"
                });

                context.CurrencyAssociations.Add(new CurrencyAssociation()
                {
                    Abbreviature = "EUR",
                    Name = "Euro"
                });

                context.SaveChanges();
            }
            var operation = new BasicOperation(context);
            var informator = new Informer(operation);
            var configuration = Substitute.For<IConfiguration>();
            configuration["RateLifetimeInCache"].Returns("1800000");
            configuration["MaxCountInPeriod"].Returns("10");
            configuration["ExchangeLimitedPeriodInHours"].Returns("1");
            var controller = new ExchangerService.Controllers.ExchangeController(new CachedInformer(informator, configuration), new StoryService(operation, configuration));
            return controller;
        }

        [Fact]
        public void Symbols_EmptyContext_ReturnsSuccessFalse()
        {
            var controller = GetController("Empty",false);
            var abbreviatures = new string[] { "UAH", "EUR" };
            var jsonResult = controller.Symbols(abbreviatures);
            dynamic result = JsonConvert.DeserializeObject(jsonResult);

            Assert.NotNull(result);
            Assert.False(bool.Parse(result.success.ToString()));
        }

        [Fact]
        public void Symbols_NonEmptyContext_ReturnsDecipheredAbbreviatures()
        {
            var controller = GetController("NonEmpty", true);
            var abbreviatures = new string[] { "UAH", "EUR" };
            var jsonResult = controller.Symbols(abbreviatures);
            dynamic result = JsonConvert.DeserializeObject(jsonResult);
            Assert.NotNull(result);
            Assert.True(bool.Parse(result.success.ToString()));
            Assert.Equal(result.symbols.UAH.ToString(), "Ukrainian hryvnia");
            Assert.Equal(result.symbols.EUR.ToString(), "Euro");
        }
    }
}

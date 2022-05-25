using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.Controllers;
using NUnit.Framework;
using Newtonsoft.Json;
using NSubstitute;

namespace ExchangeService.Tests
{
    [TestFixture]
    public class ExchangeControllerTests
    {
        private DateTime _startDate = DateTime.UtcNow.AddDays(-8);
        private DateTime _endDate = DateTime.UtcNow.AddDays(-1);
        private string[] _currencies = new string[] { "JPY", "USD" };

        private ExchangeController GetController()
        {
            var redirect = Substitute.For<IRedirectService>();
            var responseBody = new Response()
            {
                Result = (35m * 10).ToString(),
                Query = new
                {
                    amount = 10,
                    from = "EUR",
                    to = "UAH"
                },
                Success = true,
                Date = DateTime.UtcNow.ToString("MM/dd/yyyy")
            };
            redirect.ExchangeProcess(0, 10, "EUR", "UAH")
                .Returns(Task.FromResult(JsonConvert.SerializeObject(responseBody)));

            responseBody = new Response()
            {
                Base = "USD",
                Success = true,
                Date = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                Rates = new Dictionary<string, object>()
                {
                    {"EUR", "0.9"},
                    {"GBP", "0.7"},
                    {"UAH", "33"}
                }
            };
            redirect.LatestRatesProcess("USD", null)
                .Returns(Task.FromResult(JsonConvert.SerializeObject(responseBody)));

            responseBody = new Response()
            {
                Success = true,
                Symbols = new Dictionary<string, string>()
                {
                    { "AED", "United Arab Emirates Dirham" },
                    { "AFN", "Afghan Afghani" },
                    { "ALL", "Albanian Lek" },
                    { "AMD", "Armenian Dram" }
                }
            };

            redirect.GetAvailableCurrencies()
                .Returns(Task.FromResult(JsonConvert.SerializeObject(responseBody)));
            responseBody = new Response()
            {
                Success = true,
                TimeSeries = true,
                StartDate = _startDate.ToString("MM/dd/yyyy"),
                EndDate = _endDate.ToString("MM/dd/yyyy"),
                Base = "EUR"
            };

            redirect.RatesWithinProcess(_endDate, _startDate, "EUR", "AUD,CAD,USD")
                .Returns(JsonConvert.SerializeObject(responseBody));

            const string @base = "EUR";

            responseBody = new Response()
            {
                Success = true,
                Fluctuation = true,
                StartDate = _startDate.ToString("MM/dd/yyyy"),
                EndDate = _endDate.ToString("MM/dd/yyyy"),
                Base = "EUR",
                Rates = new Dictionary<string, object>()
                {
                    {
                        "JPY",
                        new {
                            change = 0,
                            change_pct = 1,
                            end_rate = 132,
                            start_rate = 131
                        }
                    },
                    {
                        "USD",
                        new {
                            change = 0,
                            change_pct = 0,
                            end_rate = 1,
                            start_rate = 2
                        }
                    },
                }
            };
            redirect.FluctuationProcessing(_startDate, _endDate, @base, _currencies)
                .Returns(JsonConvert.SerializeObject(responseBody));
            return new ExchangeController(redirect);
        }

        [Test]
        public void Exchange_CachedData_ReturnsCorrectResponse()
        {
            var controller = GetController();

            var response = controller.PostExchange(0, 10, "EUR", "UAH").Result;
            var responseBody = JsonConvert.DeserializeObject<Response>(response);

            Assert.That(responseBody.Success, Is.True);
            var result = Decimal.Parse(responseBody.Result);
            Assert.That(result, Is.EqualTo(10m * 35).Within(0.001m));
            Assert.That(responseBody.Date, Is.EqualTo(DateTime.UtcNow.ToString("MM/dd/yyyy")));
        }

        [Test]
        public void GetLatestRate_BaseUsdEmptySymbols_ReturnsCorrectResponse()
        {
            var controller = GetController();
            var response = controller.PostLatestRates("USD", null).Result;

            var responseBody = JsonConvert.DeserializeObject<Response>(response);

            Assert.That(responseBody.Success, Is.True);
            Assert.That(responseBody.Date, Is.EqualTo(DateTime.UtcNow.ToString("MM/dd/yyyy")));
            Assert.That(responseBody.Base, Is.EqualTo("USD"));
            Assert.That(responseBody.Rates["GBP"], Is.EqualTo("0.7"));
            Assert.That(responseBody.Rates["UAH"], Is.EqualTo("33"));
            Assert.That(responseBody.Rates["EUR"], Is.EqualTo("0.9"));
        }

        [Test]
        public void GetAvailableCurrencies_Currencies_ReturnsCorrectResponse()
        {
            var controller = GetController();
            string response = controller.PostAvailableCurrencies().Result;
            var responseBody = JsonConvert.DeserializeObject<Response>(response);
            var expectedSymbols = new Dictionary<string, object>()
            {
                { "AED", "United Arab Emirates Dirham" },
                { "AFN", "Afghan Afghani" },
                { "ALL", "Albanian Lek" },
                { "AMD", "Armenian Dram" }
            };

            Assert.That(responseBody.Symbols, Is.EquivalentTo(expectedSymbols));
            Assert.That(responseBody.Success, Is.True);
        }

        [Test]
        public void GetRatesWithin_ReturnsCorrectResponse()
        {
            var controller = GetController();
            var response = controller.PostRatesWithin(
                _endDate,
                _startDate,
                "EUR",
                "AUD,CAD,USD").Result;

            var responseBody = JsonConvert.DeserializeObject<Response>(response);
            Assert.That(responseBody.Base, Is.EqualTo("EUR"));
            Assert.That(responseBody.EndDate, Is.EqualTo(_endDate.ToString("MM/dd/yyyy")));
            Assert.That(responseBody.StartDate, Is.EqualTo(_startDate.ToString("MM/dd/yyyy")));
            Assert.That(responseBody.TimeSeries, Is.True);
        }

        [Test]
        public void Fluctuation_ReturnsCorrectResponse()
        {
            const string @base = "EUR";
            var expectedRates = new Dictionary<string, object>()
            {
                {
                    "JPY",
                    new
                    {
                        change = 0,
                        change_pct = 1,
                        end_rate = 132,
                        start_rate = 131
                    }
                },
                {
                    "USD",
                    new
                    {
                        change = 0,
                        change_pct = 0,
                        end_rate = 1,
                        start_rate = 2
                    }
                },
            };
            var expectedRatesJson = JsonConvert.SerializeObject(expectedRates);
            var controller = GetController();
            var response = controller.PostFluctuation(_startDate, _endDate, @base, _currencies).Result;
            var responseBody = JsonConvert.DeserializeObject<Response>(response);
            var ratesJson = JsonConvert.SerializeObject(responseBody.Rates);
            Assert.That(responseBody.Success, Is.True);
            Assert.That(responseBody.Base, Is.EqualTo("EUR"));
            Assert.That(responseBody.Fluctuation, Is.True);
            Assert.That(ratesJson, Is.EqualTo(expectedRatesJson));
        }
    }
}

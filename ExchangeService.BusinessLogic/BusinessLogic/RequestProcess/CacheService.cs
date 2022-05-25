using System.Collections.Concurrent;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;
using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess
{
    public class CacheService : ICacheService
    {
        private const string RateLifetimeKey = "RateLifetimeInCache";
        private readonly IHistoryService _storyService;
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CacheService(IConfiguration configuration, IHistoryService storyService)
        {
            _rateLifetimeInCache = Int32.Parse(configuration[RateLifetimeKey]);
            _storyService = storyService;
        }

        public bool IsCreatedExchangeRate(string from, string to, DateTime? date = null)
        {
            var rate = date is null 
                ? s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to) 
                : s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to && r.Date.HasValue && (r.Date - date).Value.Days == 0);

            return rate is not null && (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds < _rateLifetimeInCache;
        }

        public ExchangeRate? GetExchangeRateOrDefault(string from, string to, DateTime? date = null)
        {
            ExchangeRate? rate;

            if (date is null)
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);
            }
            else
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to && r.Date.HasValue && (r.Date - date).Value.Days == 0);
            }

            if (rate is null || (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                return null;
            }

            return rate;
        }

        public void SetExchangeRate(string from, string to, decimal rate, DateTime? date = null)
        {
            var exchangeRate = new ExchangeRate()
            {
                From = from,
                To = to,
                Date = date ?? DateTime.UtcNow,
                Rate = (double)rate
            };

            s_cachedRates[exchangeRate] = DateTime.UtcNow;
        }
        public Response GetExchangeFromCache(int userId, decimal amount, string from, string to)
        {
            var responseBody = new Response();
            ExchangeRate? exchangeRate = GetExchangeRateOrDefault(from, to);
            _storyService.StoreExchange(userId, exchangeRate);
            responseBody.Result = (exchangeRate.Rate * (double)amount).ToString();
            responseBody.Query = new
            {
                amount,
                from,
                to
            };
            responseBody.Success = true;
            responseBody.Date = exchangeRate.Date?.ToString("MM/dd/yyyy");
            return responseBody;
        }
        public bool IsCachedRatesWithin(DateTime startDate, DateTime endDate, string[]? currencies, string? @base)
        {
            if (currencies is null || @base is null)
            {
                return false;
            }

            for (var i = startDate; i < endDate; i = i.AddDays(1))
            {
                foreach (var currency in currencies)
                {
                    if (!IsCreatedExchangeRate(@base, currency, i))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public async Task<Response> GetAllRatesInRangeFromCache(DateTime endDate, DateTime startDate, string? @base, string[] currencies)
        {
            Response responseBody = new Response();

            for (var i = startDate; i <= endDate; i = i.AddDays(1))
            {
                responseBody.Rates[i.ToString("MM/dd/yyyy")] = new Dictionary<string, double>();

                foreach (var currency in currencies)
                {
                    ExchangeRate rate = GetExchangeRateOrDefault(@base, currency, i);
                    responseBody.Rates[currency] = rate.Rate;
                }
            }

            return responseBody;
        }
        public async Task<Response> GetCachedFluctuation(string baseCurrency, DateTime start, DateTime end, IEnumerable<string> currencies, Response responseBody)
        {
           
            foreach (var currency in currencies)
            {
                ExchangeRate? startRate = GetExchangeRateOrDefault(baseCurrency, currency, start);
                ExchangeRate? endRate = GetExchangeRateOrDefault(baseCurrency, currency, end);

                string jsonObject = string.Format(
                    "{ \"change\":\"{0}\" \r\n \"change_pct\":\"{1}\" \r\n \"end_rate\":\"{2}\" \r\n \"start_rate\":\"{3}\" \r\n}",
                    startRate.Rate / endRate.Rate, startRate.Rate / endRate.Rate, start, end);

                responseBody.Rates.Add(currency, jsonObject);
            }

            return responseBody;
        }

        
    }
}
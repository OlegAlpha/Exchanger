using System.Collections.Concurrent;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess
{
    public class RatesCache : IRatesCache
    {
        private const string RateLifetimeKey = "RateLifetimeInCache";
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public RatesCache(IConfiguration configuration)
        {
            _rateLifetimeInCache = Int32.Parse(configuration[RateLifetimeKey]);
        }

        public bool IsCreatedExchangeRate(string from, string to, DateTime? date = null)
        {
            ExchangeRate? rate;

            if(date is null)
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);
            }
            else
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to && r.Date.HasValue && (r.Date - date).Value.Days == 0);
            }

            if (rate is null || (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                return false;
            }

            return true;
        }

        public ExchangeRate? GetExchangeRateOrDefault(string from, string to, DateTime? date = null)
        {
            ExchangeRate? rate;

            if(date is null)
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
                Rate = rate
            };

            s_cachedRates[exchangeRate] = DateTime.UtcNow;
        }
    }
}
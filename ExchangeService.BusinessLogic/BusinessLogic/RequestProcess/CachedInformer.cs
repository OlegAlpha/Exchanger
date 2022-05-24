using System.Collections.Concurrent;
using ExchangerService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess
{
    public class CachedInformer
    {
        private const string RateLifetimeKey = "RateLifetimeInCache";
        private readonly Informer _informer;
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CachedInformer(Informer informer, IConfiguration configuration)
        {
            _informer = informer;
            _rateLifetimeInCache = Int32.Parse(configuration[RateLifetimeKey]);
        }

        public bool IsCreatedExchangeRate(string from, string to)
        {
            var rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);

            if (rate is null || (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                return false;
            }

            return true;
        }

        public ExchangeRate? GetExchangeRate(string from, string to)
        {
            var rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);

            if (rate is null || (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                return null;
            }

            return rate;
        }

        public void SetExchangeRate(string from, string to, string cachedResponse)
        {
            var exchangeRate = new ExchangeRate()
            {
                From = from,
                To = to,
                CachedResponse = cachedResponse,
            };

            s_cachedRates[exchangeRate] = DateTime.UtcNow;
        }
    }
}

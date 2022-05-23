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

        public ExchangeRate GetExchangeRate(string from, string to, DateTime? date = null)
        {
            var rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);

            if (rate is null || (DateTime.Now - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                var existingRate = _informer.GetExchangeRate(from, to, date) ?? throw new InvalidOperationException();

                s_cachedRates[existingRate] = DateTime.Now;

                return existingRate;
            }

            return rate;
        }

        public decimal GetExchangeStory(int userId)
        {
            return _informer.GetExchangeStory(userId);
        }

        public string GetAbbreviatureName(string abbreviature)
        {
            return _informer.GetAbbreviatureName(abbreviature);
        }
    }
}

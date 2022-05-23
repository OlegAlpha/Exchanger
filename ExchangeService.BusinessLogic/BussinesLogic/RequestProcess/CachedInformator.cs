using DataBaseLayer.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace IntermediateLayer.BussinesLogic.RequestProcess
{
    public class CachedInformator
    {
        private const string rateLifetimeKey = "RateLifetimeInCache";
        private readonly Informator _informator;
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CachedInformator(Informator informator, IConfiguration configuration)
        {
            _informator = informator;
            _rateLifetimeInCache = Int32.Parse(configuration[rateLifetimeKey]);
        }

        public ExchangeRate GetExchangeRate(string from, string to, DateTime? date = null)
        {
            var rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);

            if (rate is null || (DateTime.Now - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                var existingRate = _informator.GetExchangeRate(from, to, date) ?? throw new InvalidOperationException();

                s_cachedRates[existingRate] = DateTime.Now;

                return existingRate;
            }

            return rate;
        }

        public decimal GetExchangeStory(int userId)
        {
            return _informator.GetExchangeStory(userId);
        }

        public string GetAbbreviatureName(string abbreviature)
        {
            return _informator.GetAbbreviatureName(abbreviature);
        }
    }
}

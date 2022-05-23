using DataBaseLayer.Entities;
using IntermediateLayer.Models.StaticObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching;

namespace IntermediateLayer.BussinesLogic.RequestProcess
{
    public class InformatorDecorator
    {
        private static readonly string s_cacheKey = "_ExchangeRates";
        private readonly Informator _informator;
        private readonly IMemoryCache _cache;

        public InformatorDecorator(Informator informator, IMemoryCache cache)
        {
            _informator = informator;
            _cache = cache;

        }

        public ExchangeRate GetExchangeRate(string from, string to, DateTime? date = null)
        {
            Dictionary<ExchangeRate, DateTime>? rates = null;

            if (_cache.TryGetValue(s_cacheKey, out var exchangeRates))
            {
                rates = ((Dictionary<ExchangeRate, DateTime>)exchangeRates);
            }

            var rate = rates?.Keys.FirstOrDefault(r => r.From == from && r.To == to);

            if (rate is null || (DateTime.Now - rates[rate]).Milliseconds >= 60 * 60 * 1000)
            {
                var existingRate = _informator.GetExchangeRate(from, to, date);
                rates ??= new Dictionary<ExchangeRate, DateTime>();
                rates.Add(existingRate, DateTime.Now);
                _cache.Set(s_cacheKey, rates);
                return existingRate;
            }

            throw new InvalidOperationException();
        }

        public decimal GetExchangeStory(int userId)
        {
            return _informator.GetExchangeStory(userId);
        }
    }
}

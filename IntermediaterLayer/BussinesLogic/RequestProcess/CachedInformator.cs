using DataBaseLayer.Entities;
using System.Collections.Concurrent;

namespace IntermediateLayer.BussinesLogic.RequestProcess
{
    public class CachedInformator
    {
        private readonly Informator _informator;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CachedInformator(Informator informator)
        {
            _informator = informator;

        }

        public ExchangeRate GetExchangeRate(string from, string to, DateTime? date = null)
        {
            var rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);

            if (rate is null || (DateTime.Now - s_cachedRates[rate]).Milliseconds >= 60 * 60 * 1000)
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
    }
}

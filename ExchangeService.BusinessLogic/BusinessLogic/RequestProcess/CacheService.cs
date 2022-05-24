using System.Collections.Concurrent;
using System.Text;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ExchangeService;
using RestSharp;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess
{
    public class CacheService : ICacheService
    {
        private const string ApiConfigurationKey = "API_KEY";
        private const string ApiUrlKey = "API_URL";
        private const string ApiKeyHeader = "apikey";
        private readonly IHistoryService _storyService;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly ICacheService _cacheService;

        private const string RateLifetimeKey = "RateLifetimeInCache";
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CacheService(IConfiguration configuration, ICacheService cache, IHistoryService storyService)
        {
            _rateLifetimeInCache = Int32.Parse(configuration[RateLifetimeKey]);
            _apiKey = configuration[ApiConfigurationKey];
            _apiUrl = configuration[ApiUrlKey];
            _cacheService = cache;
            _storyService = storyService;
        }

        public bool IsCreatedExchangeRate(string from, string to, DateTime? date = null)
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
                return false;
            }

            return true;
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

        private async Task<Response> ReguestToExchange(int userId, decimal amount, string from, string to)
        {
            var responseBody = new Response();
            string url = new StringBuilder(_apiUrl).Append("/convert?")
                    .Append($"to={to}").Append($"&from={from}").Append($"&amount={amount}").ToString();
            var client = new RestClient($"{_apiUrl}/convert?to={to}&from={from}&amount={amount}");
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader(ApiKeyHeader, _apiKey);
            var response = await client.ExecuteGetAsync(request);

            responseBody = JsonConvert.DeserializeObject<Response>(response.Content);
            var rate = decimal.Parse(responseBody.Info.GetPropertyValue<string>("Rate"));
            _cacheService.SetExchangeRate(from, to, rate);
            ExchangeRate? exchangeRate = _cacheService.GetExchangeRateOrDefault(from, to);
            _storyService.StoreExchange(userId, exchangeRate);

            return responseBody;
        }

        private Response GetExchangeFromCache(int userId, decimal amount, string from, string to)
        {
            var responseBody = new Response();
            ExchangeRate exchangeRate = _cacheService.GetExchangeRateOrDefault(from, to);
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
        public async Task<string> ExchageProcess(int userId, decimal amount, string from, string to)
        {
            Response responseBody = new();
            try
            {
                if (_cacheService.IsCreatedExchangeRate(from, to))
                {
                    responseBody = GetExchangeFromCache(userId, amount, from, to);
                }
                else
                {
                    responseBody = await ReguestToExchange(userId, amount, from, to);
                }
            }
            catch
            {
                responseBody.Success=false;
            }

            return JsonConvert.SerializeObject(responseBody);
        }
    }
}
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

        private const string RateLifetimeKey = "RateLifetimeInCache";
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CacheService(IConfiguration configuration,IHistoryService storyService)
        {
            _rateLifetimeInCache = Int32.Parse(configuration[RateLifetimeKey]);
            _apiKey = configuration[ApiConfigurationKey];
            _apiUrl = configuration[ApiUrlKey];
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
            SetExchangeRate(from, to, rate);
            ExchangeRate? exchangeRate = GetExchangeRateOrDefault(from, to);
            _storyService.StoreExchange(userId, exchangeRate);

            return responseBody;
        }

        private Response GetExchangeFromCache(int userId, decimal amount, string from, string to)
        {
            var responseBody = new Response();
            ExchangeRate exchangeRate = GetExchangeRateOrDefault(from, to);
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
                if (IsCreatedExchangeRate(from, to))
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
                responseBody.Success = false;
            }

            return JsonConvert.SerializeObject(responseBody);
        }

        private async Task<Response> GetLatestRatesWithUncachedData(string newSymbols, string? @base, string? symbols)
        {
            Response responseBody;

            if (string.IsNullOrEmpty(newSymbols))
            {
                responseBody = new Response()
                {
                    Base = @base,
                    Date = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                    Success = true,
                    TimeStamp = DateTime.UtcNow.Millisecond.ToString()
                };
            }
            else
            {
                responseBody = await GetLatestuncachetRates(@base, newSymbols);
            }

            return responseBody;
        }

        public async Task<string> LatestRatesProcess(string? @base, string? symbols)
        {
            var toCurrencies = symbols?.Split(',').ToList();
            var currencies = new Dictionary<string, decimal>();

            toCurrencies?.ForEach(currency =>
            {
                var rate = GetExchangeRateOrDefault(@base, currency);
                if (rate is null)
                {
                    return;
                }

                if (IsCreatedExchangeRate(@base, currency))
                {
                    currencies[currency] = (decimal)GetExchangeRateOrDefault(@base, currency).Rate;
                    toCurrencies.Remove(currency);
                }
            });
            var newSymbols = String.Join(",", toCurrencies);

            Response responseBody = await GetLatestRatesWithUncachedData(newSymbols, @base, symbols);

            foreach (var kv in currencies)
            {
                responseBody.Rates[kv.Key] = kv.Value.ToString();
            }

            return JsonConvert.SerializeObject(responseBody);
        }

        private async Task<Response> GetLatestuncachetRates(string? @base, string newSymbols)
        {
            Response responseBody;

            var urlBuilder = new StringBuilder($"{_apiUrl}/latest?")
                .AppendIf($"symbols={newSymbols}&", String.IsNullOrWhiteSpace(newSymbols) == false)
                .AppendIf($"base={@base}", @base is not null);

            var client = new RestClient(urlBuilder.ToString());
            var request = new RestRequest();
            request.AddHeader(ApiKeyHeader, _apiKey);
            var response = await client.ExecuteAsync(request);
            responseBody = JsonConvert.DeserializeObject<Response>(response.Content);

            return responseBody;
        }
    }
}
using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.DataAccessLayer.Entities;
using Newtonsoft.Json;

namespace ExchangeService.BusinessLogic.BusinessLogic.CommonPatterns;
public class ServiceMediator : IRedirectService
{
    private readonly ICacheService _cacheService;
    private readonly IApiService _apiService;
    private readonly IHistoryService _historyService;

    public ServiceMediator(ICacheService cachedService, IApiService apiService, IHistoryService historyService)
    {
        _cacheService = cachedService;
        _apiService = apiService;
        _historyService = historyService;
    }

    public async Task<string> ExchangeProcess(int userId, decimal amount, string from, string to)
    {
        Response responseBody = new();
        try
        {
            if (_cacheService.IsCreatedExchangeRate(from, to))
            {
                responseBody = _cacheService.GetExchangeFromCache(userId, amount, from, to);
            }
            else
            {
                responseBody = await _apiService.PostRequestToExchange(userId, amount, from, to);


                ExchangeRate? exchangeRate = _cacheService.GetExchangeRateOrDefault(from, to);
                _cacheService.SetExchangeRate(from, to, (decimal)exchangeRate.Rate);
                _historyService.StoreExchange(userId, exchangeRate);
            }
        }
        catch
        {
            responseBody.Success = false;
        }

        return JsonConvert.SerializeObject(responseBody);
    }
    public async Task<string> LatestRatesProcess(string? @base, string? symbols)
    {
        var toCurrencies = symbols?.Split(',').ToList();
        var currencies = new Dictionary<string, decimal>();

        toCurrencies?.ForEach(currency =>
        {
            var rate = _cacheService.GetExchangeRateOrDefault(@base, currency);
            if (rate is null)
            {
                return;
            }

            if (_cacheService.IsCreatedExchangeRate(@base, currency))
            {
                currencies[currency] = (decimal)_cacheService.GetExchangeRateOrDefault(@base, currency).Rate;
                toCurrencies.Remove(currency);
            }
        });
        var newSymbols = String.Join(",", toCurrencies);

        Response responseBody = await _apiService.PostLatestRatesWithUncachedData(newSymbols, @base, symbols);

        foreach (var kv in currencies)
        {
            responseBody.Rates[kv.Key] = kv.Value.ToString();
        }

        return JsonConvert.SerializeObject(responseBody);
    }
    public async Task<string> RatesWithinProcess(DateTime endDate, DateTime startDate, string? @base, string? symbols)
    {
        string[] currencies = symbols.Split(',');

        if (!_cacheService.IsCachedRatesWithin(startDate, endDate, currencies, @base))
        {
            return await _apiService.PostAllRatesInRangeFromServer(endDate, startDate, @base, symbols);
        }

        Response responseBody = await _cacheService.GetAllRatesInRangeFromCache(endDate, startDate, @base, currencies);

        responseBody.Base = @base;
        responseBody.EndDate = endDate.ToString("yyyy-MM-dd");
        responseBody.StartDate = startDate.ToString("yyyy-MM-dd");
        responseBody.TimeSeries = true;

        return JsonConvert.SerializeObject(responseBody);
    }
    public async Task<string> FluctuationProcessing(DateTime start, DateTime end, string? baseCurrency, string[]? currencies)
    {
        var uncachedCurrencies = new List<string>();
        var cachedCurrencies = new List<string>();
        Response responseBody;

        try
        {
            if (currencies is not null && baseCurrency is not null)
            {
                foreach (string currency in currencies)
                {
                    ExchangeRate? startRate = null;
                    ExchangeRate? endRate = null;

                    if (_cacheService.IsCreatedExchangeRate(baseCurrency, currency, start))
                    {
                        startRate = _cacheService.GetExchangeRateOrDefault(baseCurrency, currency, start);
                    }

                    if (_cacheService.IsCreatedExchangeRate(baseCurrency, currency, end))
                    {
                        endRate = _cacheService.GetExchangeRateOrDefault(baseCurrency, currency, end);
                    }

                    if (startRate is null || endRate is null)
                    {
                        uncachedCurrencies.Add(currency);
                        continue;
                    }

                    cachedCurrencies.Add(currency);
                }
            }
            
            if (uncachedCurrencies.Count > 0)
            {
                responseBody = await _apiService.PostUncachedFluctuation(start, end, baseCurrency, uncachedCurrencies);
            }

            else if (currencies is null)
            {
                responseBody = await _apiService.PostUncachedFluctuation(start, end, baseCurrency, currencies);
            }
            else
            {
                responseBody = new Response()
                {
                    Base = baseCurrency,
                    EndDate = end.ToString("yyyy-MM-dd"),
                    Fluctuation = true,
                    StartDate = start.ToString("yyyy-MM-dd"),
                    Success = true,
                };
            }

            responseBody = await _cacheService.GetCachedFluctuation(baseCurrency, start, end, cachedCurrencies, responseBody);
        }
        catch
        {
            responseBody = new Response() { Success = false };
        }

        return JsonConvert.SerializeObject(responseBody);
    }
    public async Task<string?> GetAvailableCurrencies()
    {
        return await _apiService.PostAvailableCurrencies();
    }

}

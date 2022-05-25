using ExchangeService.BusinessLogic.Context;
using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;

public interface ICacheService
{
    bool IsCreatedExchangeRate(string from, string to, DateTime? date = null);
    ExchangeRate? GetExchangeRateOrDefault(string from, string to, DateTime? date = null);
    void SetExchangeRate(string from, string to, decimal rate, DateTime? date = null);
    Response GetExchangeFromCache(int userId, decimal amount, string from, string to);
    bool IsCachedRatesWithin(DateTime startDate, DateTime endDate, string[] currencies, string? @base);
    Task<Response> GetAllRatesInRangeFromCache(DateTime endDate, DateTime startDate, string? @base, string[] currencies);
    Task<Response> GetCachedFluctuation(string baseCurrency, DateTime start, DateTime end, IEnumerable<string> currencies, Response responseBody);
}
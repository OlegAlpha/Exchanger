using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public interface ICacheService
{
    bool IsCreatedExchangeRate(string from, string to, DateTime? date = null);
    ExchangeRate? GetExchangeRateOrDefault(string from, string to, DateTime? date = null);
    void SetExchangeRate(string from, string to, decimal rate, DateTime? date = null);
    public Task<string> ExchageProcess(int userId, decimal amount, string from, string to);
Task<string?> LatestRatesProcess(string? @base, string? symbols);
    Task<string?> GetAvailableCurrencies();
    Task<string?> RatesWithinProcess(DateTime endDate, DateTime startDate, string? @base, string? symbols);
    Task<string> FluctuationProcessing(DateTime start, DateTime end, string baseCurrency, string[] currencies);
}
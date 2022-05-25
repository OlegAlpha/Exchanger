namespace ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
public interface IRedirectService
{
    Task<string> ExchangeProcess(int userId, decimal amount, string from, string to);
    Task<string> LatestRatesProcess(string? @base, string? symbols);
    Task<string> RatesWithinProcess(DateTime endDate, DateTime startDate, string? @base, string? symbols);
    Task<string> FluctuationProcessing(DateTime start, DateTime end, string? baseCurrency, string[]? currencies);
    Task<string?> GetAvailableCurrencies();
}

using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public interface IRatesCache
{
    bool IsCreatedExchangeRate(string from, string to, DateTime? date = null);
    ExchangeRate? GetExchangeRateOrDefault(string from, string to, DateTime? date = null);
    void SetExchangeRate(string from, string to, decimal rate, DateTime? date = null);
}
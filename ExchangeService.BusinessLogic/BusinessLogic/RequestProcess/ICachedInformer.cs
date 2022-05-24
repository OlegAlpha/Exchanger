using ExchangerService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public interface ICachedInformer
{
    bool IsCreatedExchangeRate(string from, string to);
    ExchangeRate? GetExchangeRate(string from, string to);
    void SetExchangeRate(string from, string to, decimal rate);
}
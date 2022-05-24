using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public interface IHistoryService
{
    Task<bool> ExchangesCountIsValid(int userId);
    void StoreExchange(int userId, ExchangeRate rate);
}
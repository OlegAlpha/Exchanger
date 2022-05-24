using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public interface IStoryService
{
    bool ExchangesCountIsValid(int userId);
    void StoreExchange(int userId, ExchangeRate rate);
}
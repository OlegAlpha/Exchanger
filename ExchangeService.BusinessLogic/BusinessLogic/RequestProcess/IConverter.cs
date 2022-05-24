using ExchangerService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public interface IConverter
{
    void AddToStory(int userId, ExchangeRate exchangeRate);
    bool CheckCountExchanges(int userId);
    void Exchange(int userId, ExchangeRate rate);
}
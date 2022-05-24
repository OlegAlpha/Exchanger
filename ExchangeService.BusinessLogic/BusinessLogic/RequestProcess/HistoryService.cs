using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.BusinessLogic.Models.StaticObjects;
using ExchangeService.BusinessLogic.Models.Story;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public class HistoryService : IHistoryService
{
    private const string MaxCountInPeriodKey = "MaxCountInPeriod";
    private const string ExchangeLimitedPeriodKey = "ExchangeLimitedPeriodInHours";
    private readonly double _exchangeLimitedPeriodInHours;
    private readonly double _maxCountInPeriod;
    private readonly BasicOperation _operation;
    public HistoryService(BasicOperation operation, IConfiguration configuration)
    {
        _operation = operation;
        _maxCountInPeriod = Double.Parse(configuration[MaxCountInPeriodKey]);
        _exchangeLimitedPeriodInHours = Double.Parse(configuration[ExchangeLimitedPeriodKey]);
    }

    private void AddToHistory(int userId, ExchangeRate exchangeRate)
    {
        if (!StaticObjects.Stories.ContainsKey(userId))
        {
            var userHistory = new UserHistory(userId, _operation);

            StaticObjects.Stories.Add(userId, userHistory);
        }

        LocalExchangeHitory history = new LocalExchangeHitory()
        {
            Created = DateTime.UtcNow,
            Rate = exchangeRate,
        };

        StaticObjects.Stories[userId].ExchangeStories.Add(history);
    }

    public bool ExchangesCountIsValid(int userId)
    {
        if (StaticObjects.Stories.ContainsKey(userId) == false)
        {
            StaticObjects.Stories[userId] = new UserHistory(userId, _operation);
        }

        IEnumerable<LocalExchangeHitory> history
            = StaticObjects.Stories[userId].ExchangeStories.Where(history => (DateTime.UtcNow - history.Created).Hours >= _exchangeLimitedPeriodInHours);

        foreach (LocalExchangeHitory historyItem in history)
        {
            StaticObjects.Stories[userId].ExchangeStories.Remove(historyItem);
        }

        if (StaticObjects.Stories[userId].ExchangeStories.Count() > _maxCountInPeriod)
        {
            return false;
        }

        return true;
    }

    public void StoreExchange(int userId, ExchangeRate rate)
    {
        if (ExchangesCountIsValid(userId) == false)
        {
            throw new InvalidOperationException("Too much exchanges.");
        }
        AddToHistory(userId, rate);
    }
}

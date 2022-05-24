using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.BusinessLogic.Models.StaticObjects;
using ExchangeService.BusinessLogic.Models.Story;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public class HistoryService : IHistoryService
{
    private const string MaxCountInPeriodKey = "MaxCountInPeriod";
    private const string ExchangeLimitedPeriodKey = "ExchangeLimitedPeriodInHours";
    private readonly double _exchangeLimitedPeriodInHours;
    private readonly double _maxCountInPeriod;
    private readonly IExchangeHistoryRepository _repository;
    public HistoryService(IExchangeHistoryRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _maxCountInPeriod = Double.Parse(configuration[MaxCountInPeriodKey]);
        _exchangeLimitedPeriodInHours = Double.Parse(configuration[ExchangeLimitedPeriodKey]);
    }

    private void AddToHistory(int userId, ExchangeRate exchangeRate)
    {
        if (!StaticObjects.Stories.ContainsKey(userId))
        {
            var userHistory = new UserHistory(userId, _repository);

            StaticObjects.Stories.Add(userId, userHistory);
        }

        LocalExchangeHistory history = new LocalExchangeHistory()
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
            StaticObjects.Stories[userId] = new UserHistory(userId, _repository);
        }

        IEnumerable<LocalExchangeHistory> history
            = StaticObjects.Stories[userId].ExchangeStories.Where(history => (DateTime.UtcNow - history.Created).Hours >= _exchangeLimitedPeriodInHours);

        foreach (LocalExchangeHistory historyItem in history)
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

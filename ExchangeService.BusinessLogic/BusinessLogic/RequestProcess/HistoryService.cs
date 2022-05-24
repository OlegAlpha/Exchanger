using ExchangeService.BusinessLogic.Models.LocalAlternatives;
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
        ExchangeHistory history = new ExchangeHistory()
        {
            UserId = userId,
            Created = DateTime.UtcNow,
            Rate = exchangeRate,
        };
        _repository.Add(history);
        
    }

    public bool ExchangesCountIsValid(int userId)
    {
        var history = _repository.FindByUserIdOrDefault(userId);
        var filteredHistory = history.Where((userRate) => (DateTime.UtcNow - userRate.Created).Hours < _exchangeLimitedPeriodInHours);

        if (filteredHistory.Count() > _maxCountInPeriod)
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

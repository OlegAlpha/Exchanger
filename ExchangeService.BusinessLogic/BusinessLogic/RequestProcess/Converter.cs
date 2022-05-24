using ExchangerService.DataAccessLayer.CRUD;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.BusinessLogic.Models.StaticObjects;
using ExchangeService.BusinessLogic.Models.Story;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
public class Converter
{
    private const string MaxCountInPeriodKey = "MaxCountInPeriod";
    private const string ExchangeLimitedPeriodKey = "ExchangeLimitedPeriodInHours";
    private readonly double _exchangeLimitedPeriodInHours;
    private readonly double _maxCountInPeriod;
    private readonly BasicOperation _operation;
    public Converter(BasicOperation operation, IConfiguration configuration)
    {
        _operation = operation;
        _maxCountInPeriod = Double.Parse(configuration[MaxCountInPeriodKey]);
        _exchangeLimitedPeriodInHours = Double.Parse(configuration[ExchangeLimitedPeriodKey]);
    }

    private void AddToStory(int userId, decimal amount, ExchangeRate exchangeRate)
    {
        if (!StaticObjects.Stories.ContainsKey(userId))
        {
            var userStory = new UserStory(userId, _operation);

            StaticObjects.Stories.Add(userId, userStory);
        }

        LocalExchangeStory story = new LocalExchangeStory()
        {
            Amount = amount,
            Created = DateTime.UtcNow,
            Rate = exchangeRate,
        };

        StaticObjects.Stories[userId].ExchangeStories.Add(story);
    }

    private bool CheckCountExchanges(int userId)
    {
        if (StaticObjects.Stories.ContainsKey(userId) == false)
        {
            StaticObjects.Stories[userId] = new UserStory(userId, _operation);
        }

        IEnumerable<LocalExchangeStory> story
            = StaticObjects.Stories[userId].ExchangeStories.Where(story => (DateTime.UtcNow - story.Created).Hours >= _exchangeLimitedPeriodInHours);

        foreach (LocalExchangeStory storyItem in story)
        {
            StaticObjects.Stories[userId].ExchangeStories.Remove(storyItem);
        }

        if (StaticObjects.Stories[userId].ExchangeStories.Count() > _maxCountInPeriod)
        {
            throw new Exception("Too much exchanges");
        }

        return true;
    }

    public decimal Exchange(int userId, decimal amount, ExchangeRate rate)
    {
        if (amount < 0)
        {
            string message = string.Format("amount for exchange lower then 0 (amount = {0})", amount);
            throw new ArgumentException(message);
        }

        CheckCountExchanges(userId);

        decimal result;

        result = rate.Rate * amount;
        AddToStory(userId, amount, rate);

        return result;
    }
}

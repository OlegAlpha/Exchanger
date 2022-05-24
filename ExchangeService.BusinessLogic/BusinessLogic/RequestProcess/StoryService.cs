﻿using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.BusinessLogic.Models.StaticObjects;
using ExchangeService.BusinessLogic.Models.Story;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;

public class StoryService : IStoryService
{
    private const string MaxCountInPeriodKey = "MaxCountInPeriod";
    private const string ExchangeLimitedPeriodKey = "ExchangeLimitedPeriodInHours";
    private readonly double _exchangeLimitedPeriodInHours;
    private readonly double _maxCountInPeriod;
    private readonly BasicOperation _operation;
    public StoryService(BasicOperation operation, IConfiguration configuration)
    {
        _operation = operation;
        _maxCountInPeriod = Double.Parse(configuration[MaxCountInPeriodKey]);
        _exchangeLimitedPeriodInHours = Double.Parse(configuration[ExchangeLimitedPeriodKey]);
    }

    private void AddToStory(int userId, ExchangeRate exchangeRate)
    {
        if (!StaticObjects.Stories.ContainsKey(userId))
        {
            var userStory = new UserStory(userId, _operation);

            StaticObjects.Stories.Add(userId, userStory);
        }

        LocalExchangeStory story = new LocalExchangeStory()
        {
            Created = DateTime.UtcNow,
            Rate = exchangeRate,
        };

        StaticObjects.Stories[userId].ExchangeStories.Add(story);
    }

    public bool ExchangesCountIsValid(int userId)
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
        AddToStory(userId, rate);
    }
}

using DataBaseLayer.Entities;
using IntermediateLayer.Models;
using IntermediateLayer.Models.LocalivesAlternatives;
using IntermediateLayer.Models.StaticObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.BussinesLogic.RequestProcess;
public class Converter
{
    private void AddToStory(int UserId, decimal amount, ExchangeRate exchangeRate)
    {
        if (!StaticObjects.Stories.ContainsKey(UserId))
        {
            UserStory userStory = new UserStory(UserId);

            StaticObjects.Stories.Add(UserId, userStory);
        }

        LocalExchangeStory story = new LocalExchangeStory()
        {
            Amount = amount,
            Created = DateTime.UtcNow,
            Rate = exchangeRate,
        };

        StaticObjects.Stories[UserId].ExchangeStories.Add(story);
    }

    private bool CheckCountExchanges(int userId)
    {
        IEnumerable<LocalExchangeStory> story
            = StaticObjects.Stories[userId].ExchangeStories.Where(story => (DateTime.UtcNow - story.Created).Hours >= 1);

        if(story is null)
        {
            return true;
        }

        foreach (LocalExchangeStory storyItem in story)
        {
            StaticObjects.Stories[userId].ExchangeStories.Remove(storyItem);
        }

        if (StaticObjects.Stories[userId].ExchangeStories.Count() > 10)
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

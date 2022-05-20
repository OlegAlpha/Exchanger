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

    public decimal Exchange(int UserId, decimal amount, ExchangeRate rate)
    {
        if (amount < 0)
        {
            string message = string.Format("amount for exchange lower then 0 (amount = {0})", amount);
            throw new ArgumentException(message);
        }

        decimal result;

        result = rate.Rate * amount;
        AddToStory(UserId, amount, rate);

        return result;
    }
}

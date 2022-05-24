using ExchangerService.DataAccessLayer.CRUD;
using ExchangerService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
public class Informer
{
    private readonly BasicOperation _operation;
    public Informer(BasicOperation operation)
    {
        _operation = operation;
    }

    public ExchangeRate GetExchangeRate(string from, string to, DateTime? date = null)
    {
        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentNullException($"{nameof(GetExchangeRate)} has argument which is null ({nameof(from)} or {nameof(to)})");
        }

        ExchangeRate? result = _operation.FindOrDefault(Find);

        if(result == null)
        { 
            throw new ArgumentException($"this Exchange rate was not created {DateTime.UtcNow.Date} with so parameters {from} and {to}");
        }

        return result;

        bool Find(ExchangeRate exchangeRate)
        {
            if(exchangeRate is null)
            {
                return false;
            }

            if(date is null)
            {
                date = DateTime.UtcNow.Date;
            }

            bool exchange = exchangeRate.From == from && exchangeRate.To == to;
            bool lastDate =  (date.Value - exchangeRate.Created).Days == 0;

            return exchange && lastDate;
        }
    }

    public string GetAbbreviatureName(string abbreviature)
    {
        if (string.IsNullOrEmpty(abbreviature))
        {
            throw new ArgumentNullException(nameof(abbreviature));
        }

        return _operation.ReadAbbreviatureAssociation(abbreviature);
    }

    public decimal GetExchangeStory(int userId)
    {
        throw new NotImplementedException();
    }
}

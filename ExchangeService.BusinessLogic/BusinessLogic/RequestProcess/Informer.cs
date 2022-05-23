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
            string message = string.Format("{0} has argument which is null ({1} or {2})", nameof(GetExchangeRate), nameof(from), nameof(to));

            throw new ArgumentNullException(message);
        }

        ExchangeRate? result = _operation.FindOrDefault(Find);

        if(result == null)
        {
            string message = string.Format("this Exchange rate was not created {0} with so parameters {1} and {2}", DateTime.UtcNow.Date, from, to);
            throw new ArgumentException();
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

using DataBaseLayer.CRUD;
using DataBaseLayer.Entities;
using IntermediateLayer.Models.StaticObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseLayer;

namespace IntermediateLayer.BussinesLogic.RequestProcess;
public class Informator
{
    private readonly BasicOperation _operation;
    public Informator(BasicOperation operation)
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

        ExchangeRate? result = _operation.FindOrDefault(find);

        if(result == null)
        {
            string message = string.Format("this Exchange rate was not created {0} with so parameters {1} and {2}", DateTime.UtcNow.Date, from, to);
            throw new ArgumentException();
        }

        return result;

        bool find(ExchangeRate exchangeRate)
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

    public decimal GetExchangeStory(int UserId)
    {
        throw new NotImplementedException();
    }
}

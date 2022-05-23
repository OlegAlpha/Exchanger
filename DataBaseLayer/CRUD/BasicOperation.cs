using DataBaseLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseLayer.CRUD;
public class BasicOperation
{
    private readonly Context _context;
    public BasicOperation(Context context)
    {
        _context = context;
    }

    private void Add(object entity)
    {
        _ = _context.AddAsync(entity).Result;
        _context.SaveChanges();
    }

    public async Task AddAsync(object entity)
    {
        await Task.Run(()=> Add(entity));
    }

    public ExchangeRate? FindOrDefault(Func<ExchangeRate, bool> predicate)
    {
        return _context.ExchangeRates.FirstOrDefault(predicate);
    }

    public string ReadAbbriviatureAssociation(string abbriviature)
    {
        if (string.IsNullOrEmpty(abbriviature))
        {
            throw new ArgumentNullException(nameof(abbriviature));
        }


        CurrencyAssociation result = _context.currencyAssociations?.FirstOrDefault((abbr) => abbr.Abbreviature.Equals(abbriviature));

        if (result is null)
        {
            string message = string.Format("abrriviature {0} is not exist", abbriviature);
            throw new ArgumentException(message);
        }

        return result.Name;
    }
}

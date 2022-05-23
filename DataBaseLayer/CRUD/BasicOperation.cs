using DataBaseLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseLayer.CRUD;
public class BasicOperation
{
    private static Context context = new Context();

    private void Add(object entity)
    {
        lock (context)
        {
            context.Add(entity);
            context.SaveChanges();
        }
    }

    public async Task AddAsync(object entity)
    {
        await Task.Run(() => Add(entity));
    }

    public string ReadAbbriviatureAssociation(string abbriviature)
    {
        if (string.IsNullOrEmpty(abbriviature))
        {
            throw new ArgumentNullException(nameof(abbriviature));
        }


        CurrencyAssociation result = context.currencyAssociations?.FirstOrDefault((abbr) => abbr.Abbreviature.Equals(abbriviature));

        if (result is null)
        {
            string message = string.Format("abrriviature {0} is not exist", abbriviature);
            throw new ArgumentException(message);
        }

        return result.Name;
    }
}

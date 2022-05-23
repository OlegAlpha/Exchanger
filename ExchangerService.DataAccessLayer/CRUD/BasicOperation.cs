using ExchangerService.DataAccessLayer.Entities;

namespace ExchangerService.DataAccessLayer.CRUD;
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

    public string ReadAbbreviatureAssociation(string abbreviature)
    {
        if (string.IsNullOrEmpty(abbreviature))
        {
            throw new ArgumentNullException(nameof(abbreviature));
        }


        CurrencyAssociation result = _context.CurrencyAssociations?.FirstOrDefault((abbr) => abbr.Abbreviature.Equals(abbreviature));

        if (result is null)
        {
            string message = string.Format("abrriviature {0} is not exist", abbreviature);
            throw new ArgumentException(message);
        }

        return result.Name;
    }
}

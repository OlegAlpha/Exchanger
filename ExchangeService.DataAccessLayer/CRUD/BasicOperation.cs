using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.DataAccessLayer.CRUD;
public class BasicOperation
{
    private readonly Context _context;
    public BasicOperation(Context context)
    {
        _context = context;
    }

    public void Add(object entity)
    {
        _context.Add(entity);
        _context.SaveChanges();
    }

    public ExchangeHistory? FindByUserIdOrDefault(int userId)
    {
        return _context.ExchangeHistories.FirstOrDefault(x => x.UserId == userId);
    }
}

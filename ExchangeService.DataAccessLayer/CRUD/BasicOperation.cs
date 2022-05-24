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

    public ExchangeStory? FindByUserIdOrDefault(int userId)
    {
        return _context.ExchangeStories.FirstOrDefault(x => x.UserId == userId);
    }
}

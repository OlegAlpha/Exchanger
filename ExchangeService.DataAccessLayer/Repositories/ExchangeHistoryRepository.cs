using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.DataAccessLayer.Repositories;
public class ExchangeHistoryRepository : IExchangeHistoryRepository
{
    private readonly Context _context;
    private readonly DbSet<ExchangeStory> _exchangeStories;
    public ExchangeHistoryRepository(Context context)
    {
        _context = context;
        _exchangeStories = _context.Set<ExchangeStory>();
    }

    public void Add(ExchangeStory entity)
    {
        _exchangeStories.Add(entity);
        _context.SaveChanges();
    }

    public ExchangeStory? FindByUserIdOrDefault(int userId)
    {
        return _exchangeStories.FirstOrDefault(x => x.UserId == userId);
    }
}

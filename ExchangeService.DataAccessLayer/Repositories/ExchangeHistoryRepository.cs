using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.DataAccessLayer.Repositories;
public class ExchangeHistoryRepository : IExchangeHistoryRepository
{
    private readonly Context _context;
    private readonly DbSet<ExchangeHistory> _exchangeHistories;
    public ExchangeHistoryRepository(Context context)
    {
        _context = context;
        _exchangeHistories = _context.Set<ExchangeHistory>();
    }

    public void Add(ExchangeHistory entity)
    {
        _exchangeHistories.Add(entity);
        _context.SaveChanges();
    }

    public ExchangeHistory? FindByUserIdOrDefault(int userId)
    {
        return _exchangeHistories.FirstOrDefault(x => x.UserId == userId);
    }
}

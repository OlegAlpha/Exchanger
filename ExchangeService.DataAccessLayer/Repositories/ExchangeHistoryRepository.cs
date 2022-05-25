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

    public async void Add(ExchangeHistory entity)
    {
        await _exchangeHistories.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ExchangeHistory>> FindByUserIdOrDefault(int userId)
    {
        return await _exchangeHistories.Where(x => x.UserId == userId).ToListAsync();
    }
}

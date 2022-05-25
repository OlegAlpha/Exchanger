using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.DataAccessLayer;
public class Context : DbContext
{
    public DbSet<ExchangeHistory> ExchangeHistories { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    public Context(DbContextOptions options) : base (options)
    {
        Database.EnsureCreated();
    }
}

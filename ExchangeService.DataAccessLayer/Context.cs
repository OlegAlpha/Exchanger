using ExchangerService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangerService.DataAccessLayer;
public class Context: DbContext
{
    public DbSet<ExchangeStory> ExchangeStories { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<CurrencyAssociation> CurrencyAssociations { get; set; }

    public Context(DbContextOptions options) : base (options)
    {
        Database.EnsureCreated();
    }
}

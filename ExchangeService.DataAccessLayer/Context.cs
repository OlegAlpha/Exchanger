using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.DataAccessLayer;
public class Context: DbContext
{
    public DbSet<ExchangeStory> ExchangeStories { get; set; }

    public Context(DbContextOptions options) : base (options)
    {
        Database.EnsureCreated();
    }
}

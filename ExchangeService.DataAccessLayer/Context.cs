using System.Text.Json.Serialization;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExchangeService.DataAccessLayer;
public class Context : DbContext
{
    public DbSet<ExchangeStory> ExchangeStories { get; set; }

    public Context(DbContextOptions options) : base (options)
    {
        Database.EnsureCreated();
    }
}

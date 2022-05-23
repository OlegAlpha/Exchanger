using DataBaseLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseLayer;
public class Context: DbContext
{
    public DbSet<ExchangeStory> exchangeStories { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    public Context(DbContextOptions options) : base (options)
    {
        Database.EnsureCreated();
    }
}

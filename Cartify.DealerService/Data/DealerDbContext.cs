using Microsoft.EntityFrameworkCore;
using Cartify.DealerService.Entities;

namespace Cartify.DealerService.Data;

public class DealerDbContext : DbContext
{
    public DealerDbContext(DbContextOptions<DealerDbContext> options) : base(options) { }

    public DbSet<Dealer> Dealers => Set<Dealer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dealer>().HasIndex(d => d.Email).IsUnique();
    }
}

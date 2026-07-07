using Microsoft.EntityFrameworkCore;
using Cartify.DeliveryService.Entities;

namespace Cartify.DeliveryService.Data;

public class DeliveryDbContext : DbContext
{
    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options) { }

    public DbSet<DeliveryPartner> Partners => Set<DeliveryPartner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeliveryPartner>().HasIndex(p => p.Email).IsUnique();
    }
}

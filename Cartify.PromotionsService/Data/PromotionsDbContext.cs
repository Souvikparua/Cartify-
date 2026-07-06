using Microsoft.EntityFrameworkCore;
using Cartify.PromotionsService.Entities;

namespace Cartify.PromotionsService.Data;

public class PromotionsDbContext : DbContext
{
    public PromotionsDbContext(DbContextOptions<PromotionsDbContext> options) : base(options) { }

    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<StoreSetting> Settings => Set<StoreSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coupon>().Property(c => c.Value).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Coupon>().Property(c => c.MinOrder).HasColumnType("decimal(18,2)");
        foreach (var p in new[] { "DeliveryCharge", "FreeShipThreshold", "TaxPercent", "GstPercent" })
            modelBuilder.Entity<StoreSetting>().Property(p).HasColumnType("decimal(18,2)");
    }
}

using Microsoft.EntityFrameworkCore;
using Cartify.OrderService.Entities;

namespace Cartify.OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();
        order.Property(o => o.Total).HasColumnType("decimal(18,2)");
        order.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        order.Property(o => o.Discount).HasColumnType("decimal(18,2)");
        order.Property(o => o.DeliveryCharge).HasColumnType("decimal(18,2)");
        order.Property(o => o.Tax).HasColumnType("decimal(18,2)");
    }
}

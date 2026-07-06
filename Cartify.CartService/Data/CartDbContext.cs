using Microsoft.EntityFrameworkCore;
using Cartify.CartService.Entities;

namespace Cartify.CartService.Data;

public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

    public DbSet<Cart> Carts => Set<Cart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>().HasIndex(c => c.CustomerEmail).IsUnique();
    }
}

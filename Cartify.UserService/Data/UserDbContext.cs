using Microsoft.EntityFrameworkCore;
using Cartify.UserService.Entities;

namespace Cartify.UserService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Profile> Profiles => Set<Profile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>().HasIndex(p => p.Email).IsUnique();
    }
}

using Microsoft.EntityFrameworkCore;
using Cartify.AuthService.Entities;

namespace Cartify.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}
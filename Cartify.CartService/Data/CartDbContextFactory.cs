using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cartify.CartService.Data;

public sealed class CartDbContextFactory : IDesignTimeDbContextFactory<CartDbContext>
{
    public CartDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var basePath = FindBasePath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<CartDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new CartDbContext(optionsBuilder.Options);
    }

    private static string FindBasePath()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        for (var i = 0; i < 6 && current is not null; i++)
        {
            if (File.Exists(Path.Combine(current.FullName, "appsettings.json"))) return current.FullName;
            current = current.Parent;
        }
        return Directory.GetCurrentDirectory();
    }
}

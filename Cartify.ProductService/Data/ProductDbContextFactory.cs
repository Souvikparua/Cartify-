using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cartify.ProductService.Data;

public sealed class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var basePath = FindBasePath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ProductDbContext(optionsBuilder.Options);
    }

    private static string FindBasePath()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        for (var i = 0; i < 6 && current is not null; i++)
        {
            if (File.Exists(Path.Combine(current.FullName, "appsettings.json")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        var outputPath = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 6 && outputPath is not null; i++)
        {
            if (File.Exists(Path.Combine(outputPath.FullName, "appsettings.json")))
            {
                return outputPath.FullName;
            }

            outputPath = outputPath.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}

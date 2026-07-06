using Cartify.AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed a default admin account (admin@cartify.com / Admin123!) if none exists.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
    if (!db.Users.Any(u => u.Email == "admin@cartify.com"))
    {
        db.Users.Add(new Cartify.AuthService.Entities.User
        {
            FullName = "Cartify Admin",
            Email = "admin@cartify.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Pure API microservice — the SPA is served by the API Gateway, not here.
app.MapControllers();

app.Run();

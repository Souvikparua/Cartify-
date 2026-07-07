using Cartify.AuthService.Auth;
using Cartify.AuthService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// JWT bearer authentication (validates tokens this service also issues).
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtHelper.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtHelper.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JwtHelper.SigningKey(),
            ValidateLifetime = true,
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });
builder.Services.AddAuthorization();

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
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Pure API microservice — the SPA is served by the API Gateway, not here.
app.MapControllers();

app.Run();

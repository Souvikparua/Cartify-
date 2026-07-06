using Cartify.ProductService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow the storefront / admin dashboard (served by AuthService) to call this API.
const string CorsPolicy = "CartifyFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Apply migrations on startup (retry while SQL Server warms up — needed in Docker).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    for (var attempt = 1; ; attempt++)
    {
        try { db.Database.Migrate(); break; }
        catch when (attempt < 12) { Thread.Sleep(5000); }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(CorsPolicy);

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

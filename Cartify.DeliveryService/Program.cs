using Cartify.DeliveryService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
    for (var attempt = 1; ; attempt++)
    {
        try { db.Database.Migrate(); break; }
        catch when (attempt < 12) { Thread.Sleep(5000); }
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

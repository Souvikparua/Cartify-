var builder = WebApplication.CreateBuilder(args);

// API Gateway: reverse-proxy /api/* to the microservices (config in appsettings.json).
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Serve the single-page storefront/admin/account/checkout from the gateway (edge).
app.UseDefaultFiles();
app.UseStaticFiles();

// Route /api/{service}/* to the matching microservice.
app.MapReverseProxy();

// Client-side fallback for any non-API, non-file route.
app.MapFallbackToFile("index.html");

app.Run();

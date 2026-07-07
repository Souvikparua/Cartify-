using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// API Gateway: reverse-proxy /api/* to the microservices (config in appsettings.json).
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Rate limiting — fixed window per client IP (blocks floods / brute-force with 429).
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 200,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            }));
});

var app = builder.Build();

// Security headers (XSS hardening, clickjacking protection, MIME-sniffing, referrer, CSP).
app.Use(async (ctx, next) =>
{
    var h = ctx.Response.Headers;
    h["X-Content-Type-Options"] = "nosniff";
    h["X-Frame-Options"] = "DENY";
    h["Referrer-Policy"] = "no-referrer";
    h["X-XSS-Protection"] = "1; mode=block";
    h["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline'; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src https://fonts.gstatic.com data:; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'";
    await next();
});

app.UseRateLimiter();

// Serve the single-page storefront/admin/account/checkout from the gateway (edge).
app.UseDefaultFiles();
app.UseStaticFiles();

// Route /api/{service}/* to the matching microservice.
app.MapReverseProxy();

// Client-side fallback for any non-API, non-file route.
app.MapFallbackToFile("index.html");

app.Run();

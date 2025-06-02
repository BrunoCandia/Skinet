using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    ////var env = builder.Environment;

    ////var redisConnectionString = env.IsDevelopment() ? "RedisDev" : "RedisProd";

    ////var connectionString = builder.Configuration.GetConnectionString(redisConnectionString);

    var connectionString = builder.Configuration.GetConnectionString("Redis");

    Console.WriteLine("Redis: " + connectionString);

    if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("Cannot get redis connection string");

    var configuration = ConfigurationOptions.Parse(connectionString, true);
    ////configuration.IncludePerformanceCountersInExceptions = true;
    ////configuration.IncludeDetailInExceptions = true;

    ////return ConnectionMultiplexer.Connect(configuration);

    var multiplexer = ConnectionMultiplexer.Connect(configuration);

    ////multiplexer.ConnectionFailed += (s, e) =>
    ////{
    ////    Console.WriteLine($"ConnectionFailed: {e.EndPoint} – {e.FailureType} – {e.Exception?.Message}");
    ////};

    ////multiplexer.InternalError += (s, e) =>
    ////{
    ////    Console.WriteLine($"InternalError: {e.Exception?.Message}");
    ////};

    ////multiplexer.ConfigurationChanged += (s, e) =>
    ////{
    ////    Console.WriteLine($"Config changed: {e.EndPoint}");
    ////};

    return multiplexer;
});

builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddSignalR();

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<StoreContext>();

builder.Services.AddCors();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors(x => x.AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials()
                               .WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapControllers();

app.MapGroup("api")
   .MapIdentityApi<User>();

app.MapHub<NotificationHub>("/hub/notifications");

app.MapFallbackToController("Index", "Fallback");

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var storeContext = services.GetRequiredService<StoreContext>();

    await storeContext.Database.MigrateAsync();

    await StoreContextSeed.SeedAsync(storeContext);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

await app.RunAsync();

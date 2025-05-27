using API.Middleware;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
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
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis");

    if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("Cannot get redis connection string");

    var configuration = ConfigurationOptions.Parse(connectionString, true);

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();

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

app.UseAuthorization();

app.MapControllers();

app.MapGroup("api")
   .MapIdentityApi<User>();

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

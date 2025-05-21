using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ShoppingCartService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteShoppingCartAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<ShoppingCart?> GetShoppingCartAsync(string key)
        {
            var data = await _database.StringGetAsync(key);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<ShoppingCart>(data!);
        }

        public async Task<ShoppingCart?> SetShoppingCartAsync(ShoppingCart shoppingCart)
        {
            var isCreated = await _database.StringSetAsync(shoppingCart.Id, JsonSerializer.Serialize(shoppingCart), TimeSpan.FromDays(30));

            if (!isCreated)
            {
                return null;
            }

            return await GetShoppingCartAsync(shoppingCart.Id);
        }
    }
}

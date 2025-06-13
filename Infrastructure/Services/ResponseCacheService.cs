using Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase(1);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentException("Cache key must not be null or empty.", nameof(cacheKey));

            var cachedResponse = await _database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty)
            {
                return null;
            }

            return cachedResponse;
        }

        public async Task RemoveCachedResponseByPatternAsync(string pattern)
        {
            // api/products|

            var endPoint = _redis.GetEndPoints().FirstOrDefault();

            if (endPoint is not null)
            {
                Console.WriteLine($"Removing cache by pattern: {pattern} on endpoint: {endPoint}");

                var server = _redis.GetServer(endPoint);
                var keys = server.Keys(database: 1, pattern: $"*{pattern}*").ToArray();

                if (keys.Length > 0)
                {
                    await _database.KeyDeleteAsync(keys);
                }
            }
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentException("Cache key must not be null or empty.", nameof(cacheKey));

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var serializedResponse = JsonSerializer.Serialize(response, options);

            await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
        }
    }
}

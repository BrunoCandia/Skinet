namespace Core.Interfaces
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCachedResponseAsync(string cacheKey);
        Task RemoveCachedResponseByPatternAsync(string pattern);
    }
}

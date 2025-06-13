using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace API.RequestHelper
{
    [AttributeUsage(AttributeTargets.All)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CacheAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKey(context.HttpContext.Request);

            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };

                context.Result = contentResult;

                return;
            }

            var executedContext = await next();

            if (executedContext.Result is ObjectResult objectResult && objectResult.Value is not null)
            {
                // Cache the response
                await cacheService.SetCacheResponseAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private static string GenerateCacheKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            foreach (var query in request.Query.OrderBy(q => q.Key))
            {
                keyBuilder.Append($"|{query.Key}-{query.Value}");
            }

            return keyBuilder.ToString();
        }
    }
}

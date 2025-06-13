using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelper
{
    [AttributeUsage(AttributeTargets.All)]
    public class InvalidateCacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _pattern;

        public InvalidateCacheAttribute(string pattern)
        {
            _pattern = pattern;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resutlContext = await next();

            if (resutlContext.Exception is null || resutlContext.ExceptionHandled)
            {
                var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
                await cacheService.RemoveCachedResponseByPatternAsync(_pattern);
            }
            else
            {
                // Log the exception or handle it as needed
                resutlContext.ExceptionHandled = true;
            }
        }
    }
}

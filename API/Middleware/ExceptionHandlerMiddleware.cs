using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlerMiddleware(IHostEnvironment environment, RequestDelegate next)
        {
            _environment = environment;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex, _environment);
            }
        }

        private static Task HandleException(HttpContext httpContext, Exception ex, IHostEnvironment environment)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = environment.IsDevelopment()
                    ? new ApiErrorResponse(httpContext.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiErrorResponse(httpContext.Response.StatusCode, ex.Message, "Internal server error!!!");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            return httpContext.Response.WriteAsync(json);
        }
    }
}

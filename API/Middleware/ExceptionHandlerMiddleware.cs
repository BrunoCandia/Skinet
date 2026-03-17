using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(IHostEnvironment environment, RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _environment = environment;
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (OperationCanceledException ex)
            {
                // TODO: Handle this in the client side

                await HandleOperationCanceledException(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        private async Task HandleOperationCanceledException(HttpContext httpContext, OperationCanceledException ex)
        {
            // Log the exception as per S6667 diagnostic
            _logger.LogInformation(ex, "Request was cancelled by the client.");

            // 499 is unofficial, but used for client cancellations
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = 499;
                await httpContext.Response.WriteAsync("The request was cancelled by the client.");
            }
        }

        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started, the error handler will not be executed.");
                await Task.CompletedTask;
            }

            _logger.LogError(ex, "An unhandled exception occurred.");

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _environment.IsDevelopment()
                    ? new ApiErrorResponse(httpContext.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiErrorResponse(httpContext.Response.StatusCode, ex.Message, "Internal server error!!!");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            await httpContext.Response.WriteAsync(json);
        }
    }
}

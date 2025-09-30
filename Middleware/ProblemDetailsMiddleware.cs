using System.Net;
using System.Text.Json;

namespace AgroScan.API.Middleware
{
    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProblemDetailsMiddleware> _logger;

        public ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var problemDetails = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request.",
                Status = GetStatusCode(exception),
                Detail = GetDetail(exception),
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow,
                Errors = GetErrors(exception)
            };

            context.Response.StatusCode = problemDetails.Status;

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                FileNotFoundException => (int)HttpStatusCode.NotFound,
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }

        private static string GetDetail(Exception exception)
        {
            return exception switch
            {
                ArgumentException => "The request contains invalid parameters.",
                UnauthorizedAccessException => "You are not authorized to perform this action.",
                FileNotFoundException => "The requested resource was not found.",
                NotImplementedException => "This feature is not yet implemented.",
                TimeoutException => "The request timed out.",
                _ => "An internal server error occurred."
            };
        }

        private static object? GetErrors(Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                return aggregateException.InnerExceptions.Select(e => new
                {
                    Message = e.Message,
                    Type = e.GetType().Name
                });
            }

            return new[]
            {
                new
                {
                    Message = exception.Message,
                    Type = exception.GetType().Name
                }
            };
        }
    }
}

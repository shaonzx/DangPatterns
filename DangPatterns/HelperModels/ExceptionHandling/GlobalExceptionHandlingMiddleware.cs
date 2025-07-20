using System.Text.Json;

namespace DangPatterns.HelperModels.ExceptionHandling
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.TraceIdentifier;

            // ====================================================
            // STRUCTURED LOGGING WITH CONTEXT
            // ====================================================
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["RequestPath"] = context.Request.Path.Value ?? "",
                ["RequestMethod"] = context.Request.Method,
                ["RequestQuery"] = context.Request.QueryString.ToString(),
                ["UserAgent"] = context.Request.Headers["User-Agent"].ToString(),
                ["RemoteIP"] = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                ["UserId"] = context.User?.Identity?.Name ?? "Anonymous" // If you have authentication
            }))
            {
                // Log with different levels based on exception type
                var logLevel = GetLogLevel(exception);
                _logger.Log(logLevel, exception,
                    "Unhandled exception occurred: {ExceptionType} - {ExceptionMessage}",
                    exception.GetType().Name,
                    exception.Message);
            }

            // Create response
            var response = new ApiErrorResponse
            {
                CorrelationId = correlationId,
                Message = GetUserFriendlyMessage(exception),
                StatusCode = GetStatusCode(exception),
                Timestamp = DateTimeOffset.UtcNow,
                Path = context.Request.Path,
                Method = context.Request.Method,
                Details = _environment.IsDevelopment() ? GetDeveloperDetails(exception) : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            }));
        }

        // ====================================================
        // EXCEPTION HANDLING LOGIC
        // ====================================================

        private static LogLevel GetLogLevel(Exception exception)
        {
            return exception switch
            {
                ArgumentException or ArgumentNullException => LogLevel.Warning,
                KeyNotFoundException => LogLevel.Warning,
                UnauthorizedAccessException => LogLevel.Warning,
                NotImplementedException => LogLevel.Error,
                TimeoutException => LogLevel.Error,
                _ => LogLevel.Error
            };
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => "Required information is missing",
                ArgumentException => "Invalid request parameters provided",
                KeyNotFoundException => "The requested resource was not found",
                UnauthorizedAccessException => "You don't have permission to access this resource",
                NotImplementedException => "This feature is not yet available",
                TimeoutException => "The request timed out. Please try again",
                InvalidOperationException => "The requested operation cannot be completed at this time",
                _ => "An unexpected error occurred. Please contact support if the problem persists"
            };
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => 400, // Bad Request
                ArgumentException => 400, // Bad Request
                KeyNotFoundException => 404, // Not Found
                UnauthorizedAccessException => 401, // Unauthorized
                NotImplementedException => 501, // Not Implemented
                TimeoutException => 408, // Request Timeout
                InvalidOperationException => 422, // Unprocessable Entity
                _ => 500 // Internal Server Error
            };
        }

        private static object GetDeveloperDetails(Exception exception)
        {
            return new
            {
                ExceptionType = exception.GetType().FullName,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException?.Message,
                Data = exception.Data.Count > 0 ? exception.Data.Cast<object>().ToArray() : null,
                Source = exception.Source,
                TargetSite = exception.TargetSite?.Name
            };
        }
    }
}

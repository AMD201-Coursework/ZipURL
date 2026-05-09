using System.Net;
using System.Text.Json;

namespace ZipURL.Services.Identity.Common;

/// Middleware for global exception handling

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ApplicationException ex)
        {
            // Business logic error (thrown by us)
            _logger.LogWarning(ex, "Business error");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(response);
        }
        catch (Exception ex)
        {
            // Unexpected error
            _logger.LogError(ex, "Unexpected error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new { error = "Error occurred in the system" });
            await context.Response.WriteAsync(response);
        }
    }
}
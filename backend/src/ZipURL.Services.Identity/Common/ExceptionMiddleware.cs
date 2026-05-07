using System.Net;
using System.Text.Json;

namespace ZipURL.Services.Identity.Common;

/// <summary>
/// Middleware bắt exception toàn cục
/// Tránh lộ stack trace ra client
/// </summary>
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
            // Lỗi do logic nghiệp vụ (mình throw)
            _logger.LogWarning(ex, "Business error");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(response);
        }
        catch (Exception ex)
        {
            // Lỗi không mong đợi
            _logger.LogError(ex, "Unexpected error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new { error = "Đã xảy ra lỗi hệ thống" });
            await context.Response.WriteAsync(response);
        }
    }
}
using System.Diagnostics;
using System.Text.Json;

namespace UnitConversion.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // FluentValidation 400 responses are returned from controllers and do not throw; only unhandled exceptions become 500 here.
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
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = "https://httpstatuses.com/500",
            title = "An error occurred",
            status = 500,
            detail = "The server encountered an unexpected error.",
            instance = context.Request.Path.Value,
            traceId
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
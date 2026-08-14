using System.Text.Json;
using FluentValidation;

namespace WalkInRadius.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch(ValidationException ex)
        {
            _logger.LogWarning("Validation failedL {Message}", ex.Message);
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid argument: {Message}", ex.Message);
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Invalid request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Operation failed: {Message}", ex.Message);
            await WriteErrorResponse(context, StatusCodes.Status502BadGateway, "Upstream service error", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred", null);
        }
    }
    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string error,
        string? detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error,
            detail,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

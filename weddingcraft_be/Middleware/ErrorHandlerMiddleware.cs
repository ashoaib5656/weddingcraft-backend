using System.Text.Json;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Exceptions;

namespace weddingcraft_be.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (AppException ex)
        {
            _logger.LogWarning("Domain exception [{StatusCode}]: {Message}", ex.StatusCode, ex.Message);

            ctx.Response.StatusCode = ex.StatusCode;
            ctx.Response.ContentType = "application/json";
            
            var response = ApiResponse<object>.Fail(ex.Message);
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail("An unexpected error occurred.");
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

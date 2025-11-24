using Serilog.Context;

namespace weddingcraft_be.Middleware;

public class RequestLoggingEnricherMiddleware
{
    private readonly RequestDelegate _next;
    public RequestLoggingEnricherMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var userEmail = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var endpoint = context.Request?.Path.Value ?? "";
        var ip = context.Connection?.RemoteIpAddress?.ToString() ?? "";

        using (LogContext.PushProperty("UserEmail", userEmail))
        using (LogContext.PushProperty("Endpoint", endpoint))
        using (LogContext.PushProperty("IpAddress", ip))
        {
            await _next(context);
        }
    }
}

using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        _logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

        var log = new RequestLog
        {
            Path = context.Request.Path,
            Method = context.Request.Method,
            Timestamp = DateTime.UtcNow
        };

        dbContext.RequestLogs.Add(log);
        await dbContext.SaveChangesAsync();

        await _next(context);

        _logger.LogInformation("Finished handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
    }
}

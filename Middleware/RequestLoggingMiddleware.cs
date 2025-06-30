using System.Text;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Middleware;


public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        context.Request.EnableBuffering();

        var requestBody = await ReadRequestBodyAsync(context.Request);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // Save to DB
        var logEntry = new RequestLog
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "",
            RequestBody = requestBody,
            StatusCode = context.Response.StatusCode,
            ResponseBody = responseBodyText,
            Timestamp = DateTime.UtcNow
        };

        dbContext.RequestLogs.Add(logEntry);
        await dbContext.SaveChangesAsync();

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Endpoints;

public static class RequestLogEndpoints
{
    public static void MapRequestLogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/RequestLogs").WithTags("Request/Response Logging");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var logs = await db.RequestLogs
                .OrderByDescending(r => r.Timestamp)
                .Take(100) // limit to last 100
                .ToListAsync();

            return TypedResults.Ok(logs);
        })
        .WithName("GetRequestLogs")
        .WithOpenApi()
        .WithSummary("Get last 100 Request Logs")
        .WithDescription("Get last 100 requests information including path, status code, response body...");
    }
}

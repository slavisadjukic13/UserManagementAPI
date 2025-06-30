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
        var group = routes.MapGroup("/api/RequestLogs").WithTags(nameof(RequestLog));

        group.MapGet("/", async (AppDbContext db) =>
        {
            var logs = await db.RequestLogs
                .OrderByDescending(r => r.Timestamp)
                .Take(100) // limit to last 100
                .ToListAsync();

            return TypedResults.Ok(logs);
        })
        .WithName("GetAllRequestLogs")
        .WithOpenApi();
    }
}

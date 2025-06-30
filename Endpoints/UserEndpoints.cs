using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Dtos;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
namespace UserManagementAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(User));

        group.MapGet("/", async (AppDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("Request: Get all users");
            var users = await db.Users.ToListAsync();
            logger.LogInformation("Returned {Count} users", users.Count);
            return TypedResults.Ok(users);
        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (int id, AppDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("Request: Get user by ID {Id}", id);

            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(model => model.Id == id);
            if (user is null)
            {
                logger.LogWarning("User with ID {Id} not found", id);
                return TypedResults.NotFound();
            }

            logger.LogInformation("User with ID {Id} retrieved", id);
            return TypedResults.Ok(user);
        })
        .WithName("GetUserById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, User user, AppDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("Request: Update user with ID {Id}", id);

            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.FullName, user.FullName)
                    .SetProperty(m => m.Email, user.Email)
                    .SetProperty(m => m.IsActive, user.IsActive)
                );

            if (affected == 1)
            {
                logger.LogInformation("User with ID {Id} updated successfully", id);
                return TypedResults.Ok();
            }
            else
            {
                logger.LogWarning("User with ID {Id} not found for update", id);
                return TypedResults.NotFound();
            }
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        group.MapPost("/", async (UserCreateDto userDto, IValidator < UserCreateDto > validator, AppDbContext db, ILogger<Program> logger) =>
        {
            var validationResult = await validator.ValidateAsync(userDto);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            logger.LogInformation("User created with ID {Id}", user.Id);

            return TypedResults.Created($"/api/User/{user.Id}", user);
        })
        .WithName("CreateUser")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("Request: Delete user with ID {Id}", id);

            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 1)
            {
                logger.LogInformation("User with ID {Id} deleted successfully", id);
                return TypedResults.Ok();
            }
            else
            {
                logger.LogWarning("User with ID {Id} not found for deletion", id);
                return TypedResults.NotFound();
            }
        })
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}

using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Services;

namespace ApprenticeManagement.Api.Endpoints;

public static class ApprenticeEndpoints
{
    public static void MapApprenticeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/apprentices").WithTags("Apprentices");

        group.MapGet("/", async (IApprenticeService service) =>
            Results.Ok(await service.GetAllAsync()))
            .WithName("GetAllApprentices")
            .WithSummary("Get all apprentices");

        group.MapGet("/{id:int}", async (int id, IApprenticeService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetApprenticeById")
        .WithSummary("Get an apprentice by ID");

        group.MapPost("/", async (CreateApprenticeRequest request, IApprenticeService service) =>
        {
            var result = await service.CreateAsync(request);
            return Results.Created($"/api/apprentices/{result.Id}", result);
        })
        .WithName("CreateApprentice")
        .WithSummary("Create a new apprentice");

        group.MapPut("/{id:int}", async (int id, UpdateApprenticeRequest request, IApprenticeService service) =>
        {
            var result = await service.UpdateAsync(id, request);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("UpdateApprentice")
        .WithSummary("Update an existing apprentice");

        group.MapDelete("/{id:int}", async (int id, IApprenticeService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteApprentice")
        .WithSummary("Delete an apprentice");
    }
}

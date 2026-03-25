using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Services;

namespace ApprenticeManagement.Api.Endpoints;

public static class WorkJournalEndpoints
{
    public static void MapWorkJournalEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/apprentices/{apprenticeId:int}/journal").WithTags("Work Journal");

        group.MapGet("/", async (int apprenticeId, IWorkJournalService service) =>
            Results.Ok(await service.GetByApprenticeAsync(apprenticeId)))
            .WithName("GetWorkJournal")
            .WithSummary("Get all work journal entries for an apprentice");

        group.MapGet("/{entryId:int}", async (int apprenticeId, int entryId, IWorkJournalService service) =>
        {
            var result = await service.GetByIdAsync(apprenticeId, entryId);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetWorkJournalEntry")
        .WithSummary("Get a specific work journal entry");

        group.MapPost("/", async (int apprenticeId, CreateWorkJournalEntryRequest request, IWorkJournalService service) =>
        {
            var result = await service.CreateAsync(apprenticeId, request);
            if (result is null) return Results.NotFound();
            return Results.Created($"/api/apprentices/{apprenticeId}/journal/{result.Id}", result);
        })
        .WithName("CreateWorkJournalEntry")
        .WithSummary("Add a new work journal entry for an apprentice");

        group.MapPut("/{entryId:int}", async (int apprenticeId, int entryId, UpdateWorkJournalEntryRequest request, IWorkJournalService service) =>
        {
            var result = await service.UpdateAsync(apprenticeId, entryId, request);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("UpdateWorkJournalEntry")
        .WithSummary("Update a work journal entry");

        group.MapDelete("/{entryId:int}", async (int apprenticeId, int entryId, IWorkJournalService service) =>
        {
            var deleted = await service.DeleteAsync(apprenticeId, entryId);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteWorkJournalEntry")
        .WithSummary("Delete a work journal entry");
    }
}

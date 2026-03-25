using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Services;

namespace ApprenticeManagement.Api.Endpoints;

public static class GradeEndpoints
{
    public static void MapGradeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/apprentices/{apprenticeId:int}/grades").WithTags("Grades");

        group.MapGet("/", async (int apprenticeId, IGradeService service) =>
            Results.Ok(await service.GetByApprenticeAsync(apprenticeId)))
            .WithName("GetGrades")
            .WithSummary("Get all grades for an apprentice");

        group.MapGet("/summary", async (int apprenticeId, IGradeService service) =>
        {
            var result = await service.GetSummaryAsync(apprenticeId);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetGradeSummary")
        .WithSummary("Get grade summary statistics for an apprentice");

        group.MapGet("/{gradeId:int}", async (int apprenticeId, int gradeId, IGradeService service) =>
        {
            var result = await service.GetByIdAsync(apprenticeId, gradeId);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetGradeById")
        .WithSummary("Get a specific grade");

        group.MapPost("/", async (int apprenticeId, CreateGradeRequest request, IGradeService service) =>
        {
            var result = await service.CreateAsync(apprenticeId, request);
            if (result is null) return Results.NotFound();
            return Results.Created($"/api/apprentices/{apprenticeId}/grades/{result.Id}", result);
        })
        .WithName("CreateGrade")
        .WithSummary("Add a grade for an apprentice");

        group.MapPut("/{gradeId:int}", async (int apprenticeId, int gradeId, UpdateGradeRequest request, IGradeService service) =>
        {
            var result = await service.UpdateAsync(apprenticeId, gradeId, request);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("UpdateGrade")
        .WithSummary("Update a grade");

        group.MapDelete("/{gradeId:int}", async (int apprenticeId, int gradeId, IGradeService service) =>
        {
            var deleted = await service.DeleteAsync(apprenticeId, gradeId);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteGrade")
        .WithSummary("Delete a grade");
    }
}

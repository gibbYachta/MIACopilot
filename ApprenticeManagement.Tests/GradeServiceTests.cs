using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Models;
using ApprenticeManagement.Api.Services;

namespace ApprenticeManagement.Tests;

public class GradeServiceTests
{
    private static ApprenticeDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApprenticeDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApprenticeDbContext(options);
    }

    private static async Task<int> SeedApprenticeAsync(ApprenticeDbContext db, string suffix = "")
    {
        var apprentice = new Apprentice
        {
            FirstName = "Grade",
            LastName = "Tester",
            Email = $"grade{suffix}{Guid.NewGuid()}@example.com",
            Program = "Engineering",
            StartDate = new DateOnly(2024, 1, 1)
        };
        db.Apprentices.Add(apprentice);
        await db.SaveChangesAsync();
        return apprentice.Id;
    }

    [Fact]
    public async Task GetByApprenticeAsync_ReturnsEmpty_WhenNoGrades()
    {
        using var db = CreateDbContext(nameof(GetByApprenticeAsync_ReturnsEmpty_WhenNoGrades));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);

        var result = await service.GetByApprenticeAsync(apprenticeId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenApprenticeNotFound()
    {
        using var db = CreateDbContext(nameof(CreateAsync_ReturnsNull_WhenApprenticeNotFound));
        var service = new GradeService(db);
        var request = new CreateGradeRequest("Math", 85, 100, new DateOnly(2024, 5, 1), null);

        var result = await service.CreateAsync(999, request);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesGrade_WithCorrectPercentage()
    {
        using var db = CreateDbContext(nameof(CreateAsync_CreatesGrade_WithCorrectPercentage));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);
        var request = new CreateGradeRequest("Physics", 75, 100, new DateOnly(2024, 5, 1), "Good effort");

        var result = await service.CreateAsync(apprenticeId, request);

        Assert.NotNull(result);
        Assert.Equal(apprenticeId, result.ApprenticeId);
        Assert.Equal("Physics", result.Subject);
        Assert.Equal(75, result.Score);
        Assert.Equal(75.0, result.Percentage);
    }

    [Fact]
    public async Task CreateAsync_CalculatesPercentageCorrectly_ForNonHundredMax()
    {
        using var db = CreateDbContext(nameof(CreateAsync_CalculatesPercentageCorrectly_ForNonHundredMax));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);
        var request = new CreateGradeRequest("Practical", 18, 25, new DateOnly(2024, 5, 1), null);

        var result = await service.CreateAsync(apprenticeId, request);

        Assert.NotNull(result);
        Assert.Equal(72.0, result.Percentage);
    }

    [Fact]
    public async Task GetSummaryAsync_ReturnsNull_WhenApprenticeNotFound()
    {
        using var db = CreateDbContext(nameof(GetSummaryAsync_ReturnsNull_WhenApprenticeNotFound));
        var service = new GradeService(db);

        var result = await service.GetSummaryAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSummaryAsync_ReturnsZeroes_WhenNoGrades()
    {
        using var db = CreateDbContext(nameof(GetSummaryAsync_ReturnsZeroes_WhenNoGrades));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);

        var result = await service.GetSummaryAsync(apprenticeId);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalGrades);
        Assert.Equal(0, result.AveragePercentage);
    }

    [Fact]
    public async Task GetSummaryAsync_CalculatesStatisticsCorrectly()
    {
        using var db = CreateDbContext(nameof(GetSummaryAsync_CalculatesStatisticsCorrectly));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);
        await service.CreateAsync(apprenticeId, new CreateGradeRequest("Math", 80, 100, new DateOnly(2024, 5, 1), null));
        await service.CreateAsync(apprenticeId, new CreateGradeRequest("Science", 60, 100, new DateOnly(2024, 5, 2), null));
        await service.CreateAsync(apprenticeId, new CreateGradeRequest("English", 90, 100, new DateOnly(2024, 5, 3), null));

        var result = await service.GetSummaryAsync(apprenticeId);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalGrades);
        Assert.Equal(76.67, result.AveragePercentage);
        Assert.Equal(90.0, result.HighestPercentage);
        Assert.Equal(60.0, result.LowestPercentage);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesGrade_WhenFound()
    {
        using var db = CreateDbContext(nameof(UpdateAsync_UpdatesGrade_WhenFound));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);
        var created = await service.CreateAsync(apprenticeId, new CreateGradeRequest(
            "History", 50, 100, new DateOnly(2024, 5, 1), null));

        var updated = await service.UpdateAsync(apprenticeId, created!.Id, new UpdateGradeRequest(
            "History", 70, 100, new DateOnly(2024, 5, 1), "Improved"));

        Assert.NotNull(updated);
        Assert.Equal(70, updated.Score);
        Assert.Equal(70.0, updated.Percentage);
        Assert.Equal("Improved", updated.Notes);
    }

    [Fact]
    public async Task DeleteAsync_DeletesGrade_WhenFound()
    {
        using var db = CreateDbContext(nameof(DeleteAsync_DeletesGrade_WhenFound));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new GradeService(db);
        var created = await service.CreateAsync(apprenticeId, new CreateGradeRequest(
            "Art", 90, 100, new DateOnly(2024, 5, 1), null));

        var deleted = await service.DeleteAsync(apprenticeId, created!.Id);
        var found = await service.GetByIdAsync(apprenticeId, created.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }
}

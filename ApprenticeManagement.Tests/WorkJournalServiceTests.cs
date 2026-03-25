using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Models;
using ApprenticeManagement.Api.Services;

namespace ApprenticeManagement.Tests;

public class WorkJournalServiceTests
{
    private static ApprenticeDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApprenticeDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApprenticeDbContext(options);
    }

    private static async Task<int> SeedApprenticeAsync(ApprenticeDbContext db)
    {
        var apprentice = new Apprentice
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"test{Guid.NewGuid()}@example.com",
            Program = "IT",
            StartDate = new DateOnly(2024, 1, 1)
        };
        db.Apprentices.Add(apprentice);
        await db.SaveChangesAsync();
        return apprentice.Id;
    }

    [Fact]
    public async Task GetByApprenticeAsync_ReturnsEmpty_WhenNoEntries()
    {
        using var db = CreateDbContext(nameof(GetByApprenticeAsync_ReturnsEmpty_WhenNoEntries));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new WorkJournalService(db);

        var result = await service.GetByApprenticeAsync(apprenticeId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenApprenticeNotFound()
    {
        using var db = CreateDbContext(nameof(CreateAsync_ReturnsNull_WhenApprenticeNotFound));
        var service = new WorkJournalService(db);
        var request = new CreateWorkJournalEntryRequest(
            new DateOnly(2024, 3, 1), "Day 1", "Worked on setup", 8.0, "Linux");

        var result = await service.CreateAsync(999, request);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesEntry_ForExistingApprentice()
    {
        using var db = CreateDbContext(nameof(CreateAsync_CreatesEntry_ForExistingApprentice));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new WorkJournalService(db);
        var request = new CreateWorkJournalEntryRequest(
            new DateOnly(2024, 3, 1), "First Day", "Attended orientation", 7.5, "Communication");

        var result = await service.CreateAsync(apprenticeId, request);

        Assert.NotNull(result);
        Assert.Equal(apprenticeId, result.ApprenticeId);
        Assert.Equal("First Day", result.Title);
        Assert.Equal(7.5, result.HoursWorked);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntryNotFound()
    {
        using var db = CreateDbContext(nameof(GetByIdAsync_ReturnsNull_WhenEntryNotFound));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new WorkJournalService(db);

        var result = await service.GetByIdAsync(apprenticeId, 999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntry_WhenFound()
    {
        using var db = CreateDbContext(nameof(UpdateAsync_UpdatesEntry_WhenFound));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new WorkJournalService(db);
        var created = await service.CreateAsync(apprenticeId, new CreateWorkJournalEntryRequest(
            new DateOnly(2024, 3, 1), "Old Title", "Old description", 6.0, null));

        var updated = await service.UpdateAsync(apprenticeId, created!.Id, new UpdateWorkJournalEntryRequest(
            new DateOnly(2024, 3, 2), "New Title", "New description", 8.0, "Docker"));

        Assert.NotNull(updated);
        Assert.Equal("New Title", updated.Title);
        Assert.Equal(8.0, updated.HoursWorked);
        Assert.Equal("Docker", updated.Skills);
    }

    [Fact]
    public async Task DeleteAsync_DeletesEntry_WhenFound()
    {
        using var db = CreateDbContext(nameof(DeleteAsync_DeletesEntry_WhenFound));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new WorkJournalService(db);
        var created = await service.CreateAsync(apprenticeId, new CreateWorkJournalEntryRequest(
            new DateOnly(2024, 3, 1), "To Delete", "Delete me", 4.0, null));

        var deleted = await service.DeleteAsync(apprenticeId, created!.Id);
        var found = await service.GetByIdAsync(apprenticeId, created.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public async Task GetByApprenticeAsync_ReturnsMostRecentFirst()
    {
        using var db = CreateDbContext(nameof(GetByApprenticeAsync_ReturnsMostRecentFirst));
        var apprenticeId = await SeedApprenticeAsync(db);
        var service = new WorkJournalService(db);
        await service.CreateAsync(apprenticeId, new CreateWorkJournalEntryRequest(
            new DateOnly(2024, 1, 1), "Oldest", "Desc", 8.0, null));
        await service.CreateAsync(apprenticeId, new CreateWorkJournalEntryRequest(
            new DateOnly(2024, 6, 1), "Newest", "Desc", 8.0, null));

        var result = (await service.GetByApprenticeAsync(apprenticeId)).ToList();

        Assert.Equal("Newest", result[0].Title);
        Assert.Equal("Oldest", result[1].Title);
    }
}

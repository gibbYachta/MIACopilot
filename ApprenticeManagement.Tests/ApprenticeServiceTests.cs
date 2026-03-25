using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Services;

namespace ApprenticeManagement.Tests;

public class ApprenticeServiceTests
{
    private static ApprenticeDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApprenticeDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApprenticeDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoApprentices()
    {
        using var db = CreateDbContext(nameof(GetAllAsync_ReturnsEmptyList_WhenNoApprentices));
        var service = new ApprenticeService(db);

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_AddsApprentice_AndReturnsResponse()
    {
        using var db = CreateDbContext(nameof(CreateAsync_AddsApprentice_AndReturnsResponse));
        var service = new ApprenticeService(db);
        var request = new CreateApprenticeRequest(
            "Jane", "Doe", "jane@example.com", "Software Engineering",
            new DateOnly(2024, 1, 1), null);

        var result = await service.CreateAsync(request);

        Assert.True(result.Id > 0);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("jane@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        using var db = CreateDbContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
        var service = new ApprenticeService(db);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsApprentice_WhenFound()
    {
        using var db = CreateDbContext(nameof(GetByIdAsync_ReturnsApprentice_WhenFound));
        var service = new ApprenticeService(db);
        var created = await service.CreateAsync(new CreateApprenticeRequest(
            "Alice", "Smith", "alice@example.com", "Plumbing",
            new DateOnly(2023, 6, 1), null));

        var result = await service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        using var db = CreateDbContext(nameof(UpdateAsync_ReturnsNull_WhenNotFound));
        var service = new ApprenticeService(db);
        var update = new UpdateApprenticeRequest(
            "X", "Y", "x@y.com", "Test", new DateOnly(2024, 1, 1), null);

        var result = await service.UpdateAsync(999, update);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesApprentice_WhenFound()
    {
        using var db = CreateDbContext(nameof(UpdateAsync_UpdatesApprentice_WhenFound));
        var service = new ApprenticeService(db);
        var created = await service.CreateAsync(new CreateApprenticeRequest(
            "Bob", "Jones", "bob@example.com", "Electrical",
            new DateOnly(2023, 1, 1), null));

        var result = await service.UpdateAsync(created.Id, new UpdateApprenticeRequest(
            "Robert", "Jones", "robert@example.com", "Electrical",
            new DateOnly(2023, 1, 1), new DateOnly(2025, 12, 31)));

        Assert.NotNull(result);
        Assert.Equal("Robert", result.FirstName);
        Assert.Equal("robert@example.com", result.Email);
        Assert.Equal(new DateOnly(2025, 12, 31), result.EndDate);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        using var db = CreateDbContext(nameof(DeleteAsync_ReturnsFalse_WhenNotFound));
        var service = new ApprenticeService(db);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_DeletesApprentice_WhenFound()
    {
        using var db = CreateDbContext(nameof(DeleteAsync_DeletesApprentice_WhenFound));
        var service = new ApprenticeService(db);
        var created = await service.CreateAsync(new CreateApprenticeRequest(
            "To", "Delete", "delete@example.com", "Test",
            new DateOnly(2024, 1, 1), null));

        var deleted = await service.DeleteAsync(created.Id);
        var found = await service.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllApprentices()
    {
        using var db = CreateDbContext(nameof(GetAllAsync_ReturnsAllApprentices));
        var service = new ApprenticeService(db);
        await service.CreateAsync(new CreateApprenticeRequest(
            "A", "One", "a1@example.com", "X", new DateOnly(2024, 1, 1), null));
        await service.CreateAsync(new CreateApprenticeRequest(
            "B", "Two", "b2@example.com", "Y", new DateOnly(2024, 1, 1), null));

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }
}

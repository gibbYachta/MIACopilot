using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Models;

namespace ApprenticeManagement.Api.Services;

public interface IWorkJournalService
{
    Task<IEnumerable<WorkJournalEntryResponse>> GetByApprenticeAsync(int apprenticeId);
    Task<WorkJournalEntryResponse?> GetByIdAsync(int apprenticeId, int entryId);
    Task<WorkJournalEntryResponse?> CreateAsync(int apprenticeId, CreateWorkJournalEntryRequest request);
    Task<WorkJournalEntryResponse?> UpdateAsync(int apprenticeId, int entryId, UpdateWorkJournalEntryRequest request);
    Task<bool> DeleteAsync(int apprenticeId, int entryId);
}

public class WorkJournalService(ApprenticeDbContext db) : IWorkJournalService
{
    public async Task<IEnumerable<WorkJournalEntryResponse>> GetByApprenticeAsync(int apprenticeId)
    {
        return await db.WorkJournalEntries
            .Where(e => e.ApprenticeId == apprenticeId)
            .OrderByDescending(e => e.Date)
            .Select(e => ToResponse(e))
            .ToListAsync();
    }

    public async Task<WorkJournalEntryResponse?> GetByIdAsync(int apprenticeId, int entryId)
    {
        var entry = await db.WorkJournalEntries
            .FirstOrDefaultAsync(e => e.ApprenticeId == apprenticeId && e.Id == entryId);
        return entry is null ? null : ToResponse(entry);
    }

    public async Task<WorkJournalEntryResponse?> CreateAsync(int apprenticeId, CreateWorkJournalEntryRequest request)
    {
        var exists = await db.Apprentices.AnyAsync(a => a.Id == apprenticeId);
        if (!exists) return null;

        var entry = new WorkJournalEntry
        {
            ApprenticeId = apprenticeId,
            Date = request.Date,
            Title = request.Title,
            Description = request.Description,
            HoursWorked = request.HoursWorked,
            Skills = request.Skills
        };

        db.WorkJournalEntries.Add(entry);
        await db.SaveChangesAsync();
        return ToResponse(entry);
    }

    public async Task<WorkJournalEntryResponse?> UpdateAsync(int apprenticeId, int entryId, UpdateWorkJournalEntryRequest request)
    {
        var entry = await db.WorkJournalEntries
            .FirstOrDefaultAsync(e => e.ApprenticeId == apprenticeId && e.Id == entryId);
        if (entry is null) return null;

        entry.Date = request.Date;
        entry.Title = request.Title;
        entry.Description = request.Description;
        entry.HoursWorked = request.HoursWorked;
        entry.Skills = request.Skills;

        await db.SaveChangesAsync();
        return ToResponse(entry);
    }

    public async Task<bool> DeleteAsync(int apprenticeId, int entryId)
    {
        var entry = await db.WorkJournalEntries
            .FirstOrDefaultAsync(e => e.ApprenticeId == apprenticeId && e.Id == entryId);
        if (entry is null) return false;

        db.WorkJournalEntries.Remove(entry);
        await db.SaveChangesAsync();
        return true;
    }

    private static WorkJournalEntryResponse ToResponse(WorkJournalEntry e) =>
        new(e.Id, e.ApprenticeId, e.Date, e.Title, e.Description, e.HoursWorked, e.Skills);
}

using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Models;

namespace ApprenticeManagement.Api.Services;

public interface IApprenticeService
{
    Task<IEnumerable<ApprenticeResponse>> GetAllAsync();
    Task<ApprenticeResponse?> GetByIdAsync(int id);
    Task<ApprenticeResponse> CreateAsync(CreateApprenticeRequest request);
    Task<ApprenticeResponse?> UpdateAsync(int id, UpdateApprenticeRequest request);
    Task<bool> DeleteAsync(int id);
}

public class ApprenticeService(ApprenticeDbContext db) : IApprenticeService
{
    public async Task<IEnumerable<ApprenticeResponse>> GetAllAsync()
    {
        return await db.Apprentices
            .Select(a => ToResponse(a))
            .ToListAsync();
    }

    public async Task<ApprenticeResponse?> GetByIdAsync(int id)
    {
        var apprentice = await db.Apprentices.FindAsync(id);
        return apprentice is null ? null : ToResponse(apprentice);
    }

    public async Task<ApprenticeResponse> CreateAsync(CreateApprenticeRequest request)
    {
        var apprentice = new Apprentice
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Program = request.Program,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        db.Apprentices.Add(apprentice);
        await db.SaveChangesAsync();
        return ToResponse(apprentice);
    }

    public async Task<ApprenticeResponse?> UpdateAsync(int id, UpdateApprenticeRequest request)
    {
        var apprentice = await db.Apprentices.FindAsync(id);
        if (apprentice is null) return null;

        apprentice.FirstName = request.FirstName;
        apprentice.LastName = request.LastName;
        apprentice.Email = request.Email;
        apprentice.Program = request.Program;
        apprentice.StartDate = request.StartDate;
        apprentice.EndDate = request.EndDate;

        await db.SaveChangesAsync();
        return ToResponse(apprentice);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var apprentice = await db.Apprentices.FindAsync(id);
        if (apprentice is null) return false;

        db.Apprentices.Remove(apprentice);
        await db.SaveChangesAsync();
        return true;
    }

    private static ApprenticeResponse ToResponse(Apprentice a) =>
        new(a.Id, a.FirstName, a.LastName, a.Email, a.Program, a.StartDate, a.EndDate);
}

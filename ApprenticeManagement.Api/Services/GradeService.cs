using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.DTOs;
using ApprenticeManagement.Api.Models;

namespace ApprenticeManagement.Api.Services;

public interface IGradeService
{
    Task<IEnumerable<GradeResponse>> GetByApprenticeAsync(int apprenticeId);
    Task<GradeResponse?> GetByIdAsync(int apprenticeId, int gradeId);
    Task<GradeResponse?> CreateAsync(int apprenticeId, CreateGradeRequest request);
    Task<GradeResponse?> UpdateAsync(int apprenticeId, int gradeId, UpdateGradeRequest request);
    Task<bool> DeleteAsync(int apprenticeId, int gradeId);
    Task<GradeSummaryResponse?> GetSummaryAsync(int apprenticeId);
}

public class GradeService(ApprenticeDbContext db) : IGradeService
{
    public async Task<IEnumerable<GradeResponse>> GetByApprenticeAsync(int apprenticeId)
    {
        return await db.Grades
            .Where(g => g.ApprenticeId == apprenticeId)
            .OrderByDescending(g => g.Date)
            .Select(g => ToResponse(g))
            .ToListAsync();
    }

    public async Task<GradeResponse?> GetByIdAsync(int apprenticeId, int gradeId)
    {
        var grade = await db.Grades
            .FirstOrDefaultAsync(g => g.ApprenticeId == apprenticeId && g.Id == gradeId);
        return grade is null ? null : ToResponse(grade);
    }

    public async Task<GradeResponse?> CreateAsync(int apprenticeId, CreateGradeRequest request)
    {
        var exists = await db.Apprentices.AnyAsync(a => a.Id == apprenticeId);
        if (!exists) return null;

        var grade = new Grade
        {
            ApprenticeId = apprenticeId,
            Subject = request.Subject,
            Score = request.Score,
            MaxScore = request.MaxScore,
            Date = request.Date,
            Notes = request.Notes
        };

        db.Grades.Add(grade);
        await db.SaveChangesAsync();
        return ToResponse(grade);
    }

    public async Task<GradeResponse?> UpdateAsync(int apprenticeId, int gradeId, UpdateGradeRequest request)
    {
        var grade = await db.Grades
            .FirstOrDefaultAsync(g => g.ApprenticeId == apprenticeId && g.Id == gradeId);
        if (grade is null) return null;

        grade.Subject = request.Subject;
        grade.Score = request.Score;
        grade.MaxScore = request.MaxScore;
        grade.Date = request.Date;
        grade.Notes = request.Notes;

        await db.SaveChangesAsync();
        return ToResponse(grade);
    }

    public async Task<bool> DeleteAsync(int apprenticeId, int gradeId)
    {
        var grade = await db.Grades
            .FirstOrDefaultAsync(g => g.ApprenticeId == apprenticeId && g.Id == gradeId);
        if (grade is null) return false;

        db.Grades.Remove(grade);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<GradeSummaryResponse?> GetSummaryAsync(int apprenticeId)
    {
        var apprentice = await db.Apprentices.FindAsync(apprenticeId);
        if (apprentice is null) return null;

        var grades = await db.Grades
            .Where(g => g.ApprenticeId == apprenticeId)
            .ToListAsync();

        if (grades.Count == 0)
            return new GradeSummaryResponse(
                apprenticeId,
                $"{apprentice.FirstName} {apprentice.LastName}",
                0, 0, 0, 0);

        var percentages = grades.Select(g => g.Percentage).ToList();
        return new GradeSummaryResponse(
            apprenticeId,
            $"{apprentice.FirstName} {apprentice.LastName}",
            Math.Round(percentages.Average(), 2),
            percentages.Max(),
            percentages.Min(),
            grades.Count);
    }

    private static GradeResponse ToResponse(Grade g) =>
        new(g.Id, g.ApprenticeId, g.Subject, g.Score, g.MaxScore, g.Percentage, g.Date, g.Notes);
}

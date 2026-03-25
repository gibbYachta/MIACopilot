namespace ApprenticeManagement.Api.DTOs;

public record CreateGradeRequest(
    string Subject,
    double Score,
    double MaxScore,
    DateOnly Date,
    string? Notes);

public record UpdateGradeRequest(
    string Subject,
    double Score,
    double MaxScore,
    DateOnly Date,
    string? Notes);

public record GradeResponse(
    int Id,
    int ApprenticeId,
    string Subject,
    double Score,
    double MaxScore,
    double Percentage,
    DateOnly Date,
    string? Notes);

public record GradeSummaryResponse(
    int ApprenticeId,
    string FullName,
    double AveragePercentage,
    double HighestPercentage,
    double LowestPercentage,
    int TotalGrades);

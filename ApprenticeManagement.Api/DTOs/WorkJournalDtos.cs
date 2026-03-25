namespace ApprenticeManagement.Api.DTOs;

public record CreateWorkJournalEntryRequest(
    DateOnly Date,
    string Title,
    string Description,
    double HoursWorked,
    string? Skills);

public record UpdateWorkJournalEntryRequest(
    DateOnly Date,
    string Title,
    string Description,
    double HoursWorked,
    string? Skills);

public record WorkJournalEntryResponse(
    int Id,
    int ApprenticeId,
    DateOnly Date,
    string Title,
    string Description,
    double HoursWorked,
    string? Skills);

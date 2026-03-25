namespace ApprenticeManagement.Api.Models;

public class Apprentice
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public ICollection<WorkJournalEntry> WorkJournalEntries { get; set; } = [];
    public ICollection<Grade> Grades { get; set; } = [];
}

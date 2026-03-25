namespace ApprenticeManagement.Api.Models;

public class WorkJournalEntry
{
    public int Id { get; set; }
    public int ApprenticeId { get; set; }
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double HoursWorked { get; set; }
    public string? Skills { get; set; }

    public Apprentice Apprentice { get; set; } = null!;
}

namespace ApprenticeManagement.Api.Models;

public class Grade
{
    public int Id { get; set; }
    public int ApprenticeId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public double Score { get; set; }
    public double MaxScore { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }

    public Apprentice Apprentice { get; set; } = null!;

    public double Percentage => MaxScore > 0 ? Math.Round(Score / MaxScore * 100, 2) : 0;
}

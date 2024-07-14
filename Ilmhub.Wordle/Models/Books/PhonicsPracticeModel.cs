namespace Ilmhub.Wordle.Models.Books;

public class PhonicsPractice
{
    public string? Name { get; set; }
    public int? UnitId { get; set; }
    public string? BookSlug { get; set; }
    public DateTimeOffset? StartedAt  { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }

    public Dictionary<string, PhonicsPracticeResult> Results { get; set; } = [];
    public int? TotalScore { get; set; }
    public TimeSpan? TotalElapsedTime { get; set; }

    public string? CurrentPracticeName { get; set; }
    public List<string>? Practices { get; set; } = [];
}

public class PhonicsPracticeResult
{
    public string? Name { get; set; }
    public int Score { get; set; }
    public double ElapsedTime { get; set; }
}
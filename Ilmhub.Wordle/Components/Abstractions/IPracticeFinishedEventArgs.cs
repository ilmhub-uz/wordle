namespace Ilmhub.Wordle.Components.Abstractions;

public interface IPracticeFinishedEventArgs
{
    string? Name { get; }
    DateTimeOffset StartedAt { get; }
    DateTimeOffset FinishedAt { get; set; }
    int Score { get; set; }
    TimeSpan ElapsedTime => FinishedAt - StartedAt;
}

public class PracticeFinishedEventArgs(string? name) : IPracticeFinishedEventArgs
{
    public string? Name { get; } = name;
    public DateTimeOffset StartedAt { get; } = DateTimeOffset.Now;
    public DateTimeOffset FinishedAt { get; set; }
    public int Score { get; set; }
}
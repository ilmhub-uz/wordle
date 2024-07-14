namespace Ilmhub.Wordle.Extensions;

public static class TimerExtensions
{
    public static System.Timers.Timer Reset(this System.Timers.Timer timer, TimeSpan interval)
    {
        timer.Stop();
        timer.Interval = interval.TotalMilliseconds;
        timer.Start();
        return timer;
    }
}
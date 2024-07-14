using Microsoft.AspNetCore.Components;

namespace Ilmhub.Wordle.Components.Abstractions;

public interface IPracticeComponent
{
    Task StartAsync();
    EventCallback<IPracticeFinishedEventArgs> OnFinished { get; set; }
}
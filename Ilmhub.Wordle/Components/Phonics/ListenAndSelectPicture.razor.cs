using System.Data;
using System.Text.Json;
using Ilmhub.Wordle.Components.Abstractions;
using Ilmhub.Wordle.Extensions;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ilmhub.Wordle.Components.Phonics;

public partial class ListenAndSelectPicture : IPracticeComponent
{
    public static string Name => "phonics.listen.and.select";

    [Inject] IJSRuntime? JS { get; set; }

    [Parameter] public EventCallback<IPracticeFinishedEventArgs> OnFinished { get; set; }
    [Parameter] public List<PhonicsWord>? Words { get; set; }

    private IPracticeFinishedEventArgs? OnFinishedArgs;
    private List<Question>? Questions = [];
    private Question? CurrentQuestion = default;
    private bool Started = false;
    private int currentIndex = 0; 
    private int currentProgress => (int) (currentIndex / (double) Questions!.Count * 100);

    protected override async Task OnParametersSetAsync()
    {
        var wordsCopy = Words?.Shuffle().ToList();
        Questions = Words?.Select(word => 
        {
            wordsCopy?.Shuffle();
            var options = wordsCopy?.Where(x => word.Name.IsSameWord(x.Name) is false).Take(2).ToList();
            options?.Add(word);
            return new Question(word, options?.Shuffle());
        }).ToList();

        await base.OnParametersSetAsync(); 
    }

    public async Task StartAsync()
    {
        Started = true;
        CurrentQuestion = Questions![currentIndex];
        OnFinishedArgs = new PracticeFinishedEventArgs(Name);
        StateHasChanged();

        await Task.CompletedTask;
    }


    private async Task Selected(PhonicsWord selected)
    {
        var isCorrect = string.Equals(
            selected.Name, 
            CurrentQuestion?.Target.Name, 
            StringComparison.OrdinalIgnoreCase);

        await InvokeSelectionSound(isCorrect);

        if(isCorrect is false)
        {
            CurrentQuestion!.Score = Math.Max(CurrentQuestion!.Score - 1, -1);
            return;
        }

        CurrentQuestion!.Score += 1;
        currentIndex += 1;

        if(currentIndex < Questions!.Count)
            CurrentQuestion = Questions[currentIndex];
        else 
        {
            Console.WriteLine("Finished");
            OnFinishedArgs!.Score = Questions.Sum(x => x.Score);
            OnFinishedArgs.FinishedAt = DateTimeOffset.Now;
            await OnFinished.InvokeAsync(OnFinishedArgs);
        }
       
        StateHasChanged();        
    }

    private async Task InvokeSelectionSound(bool isCorrect)
    {
        if(JS is not null)
            await JS.InvokeVoidAsync(isCorrect ? "correct" : "incorrect");
    }

    internal record Question(PhonicsWord Target, IEnumerable<PhonicsWord>? Options)
    {
        public int Score { get; set; }
    }
}
using System.Text.Json;
using Ilmhub.Wordle.Components.Abstractions;
using Ilmhub.Wordle.Extensions;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ilmhub.Wordle.Components.Phonics;

public partial class SayTheWord : IDisposable
{
    public static string? Name => "phonics.say.the.word";
    [Inject] public ISpeechRecognitionService SpeechRecognition { get; set; } = null!;
    [Inject] public ISpeechSynthesisService SpeechSynthesis { get; set; } = null!;
    [Inject] IJSRuntime? JS { get; set; }

    [Parameter] public EventCallback<IPracticeFinishedEventArgs> OnFinished { get; set; }
    [Parameter] public List<PhonicsWord> Words { get; set; } = [];

    private IPracticeFinishedEventArgs? OnFinishedArgs;
    private IDisposable? recognitionSubscription;
    private bool SkipQuestion = false;
    private bool Listening = false;
    private bool Started = false;
    private int currentIndex = 0;
    private int score = 0; 
    private int fails = 0;
    private int currentProgress =>  (int) (currentIndex / (double) Words!.Count * 100);
    private static TimeSpan timerInterval = TimeSpan.FromSeconds(15);
    private System.Timers.Timer recognitionTimer = new(timerInterval)
    {
        Enabled = true,
        AutoReset = false
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await SpeechRecognition.InitializeModuleAsync();
    }

    private async Task StartAsync() 
    {
        Started = true;
        OnFinishedArgs = new PracticeFinishedEventArgs(Name);
        Words.Shuffle();
        await SayInstruction();
        await StartListening();
    }

    private void StopListening()
    {
        SpeechRecognition.CancelSpeechRecognition(false);
        Listening = false;
        StateHasChanged();
    }

    private async Task StartListening()
    {
        Listening = true;
        recognitionTimer.Elapsed += (s, e) => StopListening();
        recognitionTimer.Reset(timerInterval);
        await Task.Run(() =>
        {
            recognitionTimer.Start();
            recognitionSubscription = SpeechRecognition.RecognizeSpeech(
                "en-US", 
                OnRecognized, 
                OnError, 
                OnStarted, 
                OnEnded);                
        });
    }

    private async Task SayInstruction()
    {
        var voices = await SpeechSynthesis.GetVoicesAsync();
        var voice = voices.FirstOrDefault(v => v.Name == "Google UK English Male");
        SpeechSynthesis.Speak(new()
        {
            Text = "Say the word.",
            Rate = 1,
            Volume = 1,
            Voice = voice
        });
        await Task.Delay(3000);
    }

    private void OnSkipQuestion()
    {
        SkipQuestion = false;
        NextQuestionOrFinish();
    }

    private void OnRecognized(string speech)
    {
        Console.WriteLine(speech);
        var cleanSpeech = CleanUpSpeech(speech);
        var isCorrect = cleanSpeech.Contains(Words[currentIndex].Name!);

        InvokeSelectionSound(isCorrect).ConfigureAwait(true);

        if(isCorrect is false)
        {
            if((string.IsNullOrWhiteSpace(cleanSpeech) is false && ++fails >= 3)
            || string.Equals(cleanSpeech, "skip", StringComparison.OrdinalIgnoreCase))
            {
                StopListening();
                SkipQuestion = true;
                StateHasChanged();
            }

            fails = 0;
            return;
        }


        score += 1;
        fails = 0;
        recognitionTimer.Reset(timerInterval);

        NextQuestionOrFinish();
    }

    private void NextQuestionOrFinish()
    {
        currentIndex += 1;

        Console.WriteLine(currentIndex);

        if(currentIndex == Words.Count)
        {
            StopListening();
            OnFinishedArgs!.Score = score;
            OnFinishedArgs.FinishedAt = DateTimeOffset.Now;
            OnFinished.InvokeAsync(OnFinishedArgs).ConfigureAwait(true);

            return;
        }

        if(Listening is false)
            StartListening().ConfigureAwait(true);

        StateHasChanged();
    }

    private static string CleanUpSpeech(string? speech)
        => speech?.Trim().Replace(" ", string.Empty).Replace(".", string.Empty).ToLower()
        ?? string.Empty;

    private async Task InvokeSelectionSound(bool isCorrect)
    {
        if(JS is not null)
            await JS.InvokeVoidAsync(isCorrect ? "correct" : "incorrect");
    }

    private void OnEnded()
    {
        Console.WriteLine("Ended");
        Listening = false;
        StateHasChanged();
    }

    private void OnStarted()
    {
        Console.WriteLine("Started");
        Listening = true;
        StateHasChanged();
    }

    private void OnError(SpeechRecognitionErrorEvent @event)
        => Console.WriteLine(JsonSerializer.Serialize(@event));

    public void Dispose() => recognitionSubscription?.Dispose();
}
using System.Net.Http.Json;
using System.Text.Json;
using Ilmhub.Wordle.Components.Abstractions;
using Ilmhub.Wordle.Components.Phonics;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ilmhub.Wordle.Pages.Phonics;

public partial class PhonicsPractice
{
    [Inject] public HttpClient? Client { get; set; }
    [Inject] public ILocalStorageService? LocalStorage { get; set; }

    [Parameter] public string? bookSlug { get; set; }
    [Parameter] public int unitId { get; set; }

    private string? Name = string.Empty;
    private PhonicsBook Book = new();
    private PhonicsUnit Unit = new();
    private Models.Books.PhonicsPractice Practice = new();
    private bool Started = false;
    private bool Finished = false;
    private string storageKey => 
        $"practices:{bookSlug}-{unitId}-{Name}".ToLower();

    protected override async Task OnInitializedAsync()
    {
        Name = "wahid";
        var books = await Client!.GetFromJsonAsync<List<PhonicsBook>>("books/phonics/phonics.json") ?? [];
        Book = books.FirstOrDefault(b => string.Equals(bookSlug, b.Slug, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception($"No phonics book with title {bookSlug}");

        Unit =  Book?.Units?.FirstOrDefault(u => u.Id == unitId)
            ?? throw new Exception($"No phonics Unit {unitId} with title {bookSlug}.");

        StateHasChanged();
    }


    public async Task OnPracticeFinished(IPracticeFinishedEventArgs args)
    {
        Console.WriteLine($"Finished {args.Name} with score {args.Score} in {args.ElapsedTime.TotalSeconds}s.");
        
        Practice.Results[args.Name!] = new PhonicsPracticeResult
        {
            Name = args.Name,
            Score = args.Score,
            ElapsedTime = args.ElapsedTime.TotalSeconds
        };

        if(Practice.Practices!.IndexOf(args.Name!) == Practice.Practices.Count - 1)
            await ShowFinalResultsAsync();
        else
            await ShowNextPracticeAsync();

        LocalStorage!.SetItem(storageKey, Practice);
    }

    private async Task ShowNextPracticeAsync()
    {
        var nextPracticeIndex = Practice.Practices!.IndexOf(Practice.CurrentPracticeName!) + 1;
        Practice.CurrentPracticeName = Practice.Practices[nextPracticeIndex];
        StateHasChanged();
        await Task.CompletedTask;
    }

    private async Task ShowFinalResultsAsync()
    {
        Finished = true;
        await Task.CompletedTask;
        StateHasChanged();
    }

    private async Task StartAsync()
    {
        Practice = LocalStorage?.GetItem<Models.Books.PhonicsPractice>(storageKey)
        ??  new Models.Books.PhonicsPractice
        {
            Name = Name,
            UnitId = unitId,
            BookSlug = bookSlug,
            StartedAt = DateTimeOffset.Now,
            Practices = [ ListenAndSelectPicture.Name, SayTheWord.Name ],
            CurrentPracticeName = ListenAndSelectPicture.Name
        };

        LocalStorage!.SetItem(storageKey, Practice);
        Started = true;
        await Task.CompletedTask;
    } 
}
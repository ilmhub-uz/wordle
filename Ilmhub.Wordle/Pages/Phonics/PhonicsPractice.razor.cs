using System.Net.Http.Json;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;

namespace Ilmhub.Wordle.Pages.Phonics;

public partial class PhonicsPractice
{
    [Inject] public HttpClient? Client { get; set; }
    [Parameter] public string? bookSlug { get; set; }
    [Parameter] public int unitId { get; set; }

    private PhonicsBook Book = new();
    private PhonicsUnit Unit = new();

    private Dictionary<string, string> Words = new();

    protected override async Task OnInitializedAsync()
    {
        var books = await Client!.GetFromJsonAsync<List<PhonicsBook>>("books/phonics/phonics.json") ?? [];
        Book = books.FirstOrDefault(b => string.Equals(bookSlug, b.Slug, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception($"No phonics book with title {bookSlug}");

        Unit =  Book?.Units?.FirstOrDefault(u => u.Id == unitId)
            ?? throw new Exception($"No phonics Unit {unitId} with title {bookSlug}.");

        Words = Unit.Images?.Select(i => new KeyValuePair<string, string>(
            key: Path.GetFileNameWithoutExtension(i),
            value: i))
            .ToDictionary()
            ?? throw new Exception($"Failed to extract word from unit {unitId} of book {bookSlug}.");

        StateHasChanged();
    }
}
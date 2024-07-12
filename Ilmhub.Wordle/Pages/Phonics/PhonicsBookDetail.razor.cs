using System.Net.Http.Json;
using Ilmhub.Wordle.Extensions;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;

namespace Ilmhub.Wordle.Pages.Phonics;

public partial class PhonicsBookDetail
{
    [Inject] HttpClient? Client { get; set; }
    [Parameter] public string? bookSlug { get; set; }
     private PhonicsBook Book = new();

    protected override async Task OnInitializedAsync()
    {
        var books = await Client!.GetFromJsonAsync<List<PhonicsBook>>("books/phonics/phonics.json") ?? [];
        Book = books.FirstOrDefault(b => string.Equals(bookSlug, b.Slug, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception($"No phonics book with title {bookSlug}");

        StateHasChanged();
    }
}
using System.Net.Http.Json;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;

namespace Ilmhub.Wordle.Pages.Phonics;

public partial class PhonicsBooksList
{
    [Inject] public HttpClient? Client { get; set; }
    private List<PhonicsBook> Books = [];

    protected override async Task OnInitializedAsync()
    {
        Books = await Client!.GetFromJsonAsync<List<PhonicsBook>>("books/phonics/phonics.json") ?? [];
        Console.WriteLine($"Loaded {Books.Count} books");
        StateHasChanged();
    }
}
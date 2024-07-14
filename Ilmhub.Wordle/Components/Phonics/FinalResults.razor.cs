using Ilmhub.Wordle.Extensions;
using Ilmhub.Wordle.Models.Books;
using Microsoft.AspNetCore.Components;

namespace Ilmhub.Wordle.Components.Phonics;

public partial class FinalResults 
{
    [Parameter] public PhonicsPractice Practice { get; set; } = new();

    private string GetPracticeNameFromKey(string key)
        => string.Join(' ', key.Split(".", StringSplitOptions.RemoveEmptyEntries))
            .ToCapitalCase();
}
using Ilmhub.Wordle.Extensions;

namespace Ilmhub.Wordle.Models.Books;

public class PhonicsBook
{
    public string? Title { get; set; }
    public string? Cover { get; set; }
    public string? Slug => Title?.Slugify();
    public List<PhonicsUnit>? Units { get; set; }
}
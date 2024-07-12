using Ilmhub.Wordle.Extensions;

namespace Ilmhub.Wordle.Models.Books;

public class PhonicsBook
{
    public string? Title { get; set; }
    public string? Cover { get; set; }
    public string? Slug => Title?.Slugify();
    public List<PhonicsUnit>? Units { get; set; }
}

public class PhonicsUnit
{
    public int Id { get; set; }
    public string? Topic { get; set; }
    public List<string>? Images { get; set; }
}
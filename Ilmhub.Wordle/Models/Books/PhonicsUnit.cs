namespace Ilmhub.Wordle.Models.Books;

public class PhonicsUnit
{
    public int Id { get; set; }
    public string? Topic { get; set; }
    public List<PhonicsWord>? Words { get; set; }
}
using System.Text.RegularExpressions;

namespace Ilmhub.Wordle.Extensions;

public static class StringExtensions
{
    public static string Slugify(this string phrase)
    {
        string str = phrase.ToLowerInvariant();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = Regex.Replace(str, @"\s", "-");
        str = str.Trim('-');

        return str;
    }

    public static bool IsSameWord(this string? word, string? another)
        => string.IsNullOrWhiteSpace(word) is false
        && string.Equals(word, another, StringComparison.OrdinalIgnoreCase);

    public static string ToCapitalCase(this string? text)
    {
        var result = Regex.Replace(text ?? string.Empty, @"\b(\w)", m => m.Value.ToUpper());
        return Regex.Replace(result, @"(\s(of|in|by|and)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);
    }

}
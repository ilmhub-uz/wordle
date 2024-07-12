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
}
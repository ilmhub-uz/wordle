namespace Ilmhub.Wordle.Extensions;

public static class CollectionExtensions
{
    private static readonly Random random = new();  
    public static List<T> Shuffle<T>(this List<T> list)  
    {  
        int n = list.Count;  
        while (n-- > 1) {  
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }
}
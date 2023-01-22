using System.Collections.Generic;

namespace BlockBuster.Utils;

internal static class Extensions
{
    public static IEnumerable<T> ReverseList<T>(this List<T> items)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            yield return items[i];
        }
    }
}

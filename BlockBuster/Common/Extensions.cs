using System.Collections.Generic;
using System.Linq;

namespace BlockBuster.Utils
{
    internal static class Extensions
    {
        public static IEnumerable<T> ReverseList<T>(this List<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        public static bool SequenceEquals<T>(this T[,] a, T[,] b) where T : IEnumerable<T>
        {
            return a?.Rank == b?.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());
        }
    }

}
using System.Collections.Generic;
using System.Linq;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? src) => src == null || !src.Any();
        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T>? src) => src ?? [];
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> src, System.Func<T, TKey> key)
            => src.GroupBy(key).Select(g => g.First());
    }
}

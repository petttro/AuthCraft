using System.Collections.Generic;
using System.Linq;

namespace AuthCraft.Common.Extensions;

public static class EnumerableExtensions
{
    public static string ToJoinedString<T>(this IEnumerable<T> source)
    {
        return string.Join(",", source);
    }

    public static bool Empty<T>(this IEnumerable<T> source)
    {
        return source?.Any() != true;
    }
}

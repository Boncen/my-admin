using System.Collections;

namespace MyAdmin.Core.Extensions;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
    {
        return source == null || source.Count <= 0;
    }
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        if (source.IsNullOrEmpty())
        {
            return false;
        }

        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    public static ICollection<string> ToStringCollection(this ICollection<dynamic> source)
    {
        ICollection<string> result = new List<string>();
        foreach (var item in source)
        {
            result.Add("'" + item + "'");
        }
        return result;
    }

    public static string ToSplitableString(this IEnumerable<dynamic> source, char seperator = ',')
    {
        return string.Join(seperator, source);
    }
}

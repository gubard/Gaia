using System.Runtime.CompilerServices;

namespace Gaia.Helpers;

public static class EnumerableExtension
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : class
    {
        foreach (var item in enumerable)
        {
            if (item is null)
            {
                continue;
            }

            yield return item;
        }
    }

    public static IEnumerable<T> WhereNotNullStruct<T>(this IEnumerable<T?> enumerable)
        where T : struct
    {
        foreach (var item in enumerable)
        {
            if (item is null)
            {
                continue;
            }

            yield return item.Value;
        }
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);

            yield return item;
        }
    }

    public static async ValueTask<IEnumerable<T>> ToEnumerableAsync<T>(
        this ConfiguredCancelableAsyncEnumerable<T> enumerable
    )
    {
        var result = new List<T>();

        await foreach (var item in enumerable)
        {
            result.Add(item);
        }

        return result;
    }
}

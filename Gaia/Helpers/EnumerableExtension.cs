namespace Gaia.Helpers;

public static class EnumerableExtension
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);

            yield return item;
        }
    }
}

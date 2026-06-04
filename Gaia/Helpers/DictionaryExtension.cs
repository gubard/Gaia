namespace Gaia.Helpers;

public static class DictionaryExtension
{
    public static Dictionary<TKey, TValue> Combine<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> x,
        IReadOnlyDictionary<TKey, TValue> y
    )
        where TKey : notnull
    {
        var result = new Dictionary<TKey, TValue>(x);

        foreach (var item in y)
        {
            result.Add(item.Key, item.Value);
        }

        return result;
    }

    public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(
        this Dictionary<TKey, TValue> x,
        IReadOnlyDictionary<TKey, TValue> y
    )
        where TKey : notnull
    {
        foreach (var item in y)
        {
            x.Add(item.Key, item.Value);
        }

        return x;
    }
}

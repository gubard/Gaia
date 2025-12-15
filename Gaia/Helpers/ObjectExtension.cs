using System.Runtime.CompilerServices;

namespace Gaia.Helpers;

public static class ObjectExtension
{
    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T[] array)
    {
        return array;
    }

    public static T ThrowIfNull<T>(
        this T? obj,
        [CallerArgumentExpression(nameof(obj))] string paramName = ""
    )
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return obj;
    }

    public static T? As<T>(this object? obj)
        where T : class
    {
        return obj as T;
    }

    public static T Cast<T>(this object obj)
        where T : class
    {
        return (T)obj;
    }

    public static IEnumerable<T> ToEnumerable<T>(this T obj)
        where T : class
    {
        return [obj];
    }
}

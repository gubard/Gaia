using System.Runtime.CompilerServices;

namespace Gaia.Helpers;

public static class ObjectExtension
{
    public static Memory<T> Concat<T>(this T[] array, ReadOnlyMemory<T> other)
    {
        var result = new T[array.Length + other.Length].AsMemory();
        array.CopyTo(result);
        other.Span.CopyTo(result.Span.Slice(array.Length));

        return result;
    }

    public static T[] SetItem<T>(this T[] array, int index, T value)
    {
        array[index] = value;

        return array;
    }

    public static int Sum<T>(this ReadOnlySpan<T> span, Func<T, int> selector)
    {
        var sum = 0;

        foreach (var item in span)
        {
            sum += selector(item);
        }

        return sum;
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T[] array)
    {
        return array;
    }

    public static T ThrowIfNullStruct<T>(
        this T? obj,
        [CallerArgumentExpression(nameof(obj))] string paramName = ""
    )
        where T : struct
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return obj.Value;
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

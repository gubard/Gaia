using System.Runtime.CompilerServices;

namespace Gaia.Helpers;

public static class ObjectExtension
{
    public static Memory<T> Concat<T>(
        this IEnumerable<Memory<T>> source,
        params IEnumerable<Memory<T>> items
    )
    {
        Memory<T>[] array = [.. source, .. items];
        var result = new T[array.Sum(x => x.Length)].AsMemory();

        var currentIndex = 0;

        foreach (var item in array)
        {
            item.CopyTo(result.Slice(currentIndex));
            currentIndex += item.Length;
        }

        return result;
    }

    public static Memory<T> Concat<T>(this Memory<T> source, params IEnumerable<Memory<T>> items)
    {
        Memory<T>[] array = [default, .. items];
        array[0] = source;

        return array.Concat();
    }

    public static Memory<T> Concat<T>(this ReadOnlyMemory<T> source, params Span<T> items)
    {
        var result = new T[source.Length + items.Length].AsMemory();
        source.CopyTo(result);
        items.CopyTo(result.Slice(source.Length).Span);

        return result;
    }

    public static Memory<T> Concat<T>(this IEnumerable<Memory<T>> source)
    {
        var array = source.ToArray();

        return array.Concat();
    }

    public static Memory<T> Concat<T>(this Memory<T>[] source)
    {
        var array = source.ToArray();
        var result = new T[array.Sum(x => x.Length)].AsMemory();
        var currentIndex = 0;

        foreach (var item in array)
        {
            item.CopyTo(result.Slice(currentIndex));
            currentIndex += item.Length;
        }

        return result;
    }

    public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this IEnumerable<T> source)
    {
        return source.ToArray();
    }

    public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] source)
    {
        return source;
    }

    public static Memory<TResult> SelectAsSpan<TTaget, TResult>(
        this ReadOnlyMemory<TTaget> source,
        Func<TTaget, TResult> selector
    )
    {
        if (source.IsEmpty)
        {
            return Memory<TResult>.Empty;
        }

        var result = new TResult[source.Length].AsMemory();

        for (var i = 0; i < source.Length; i++)
        {
            result.Span[i] = selector(source.Span[i]);
        }

        return result;
    }

    public static Memory<TResult> SelectAsSpan<TTaget, TResult>(
        this Memory<TTaget> source,
        Func<TTaget, TResult> selector
    )
    {
        if (source.IsEmpty)
        {
            return Memory<TResult>.Empty;
        }

        var result = new TResult[source.Length].AsMemory();

        for (var i = 0; i < source.Length; i++)
        {
            result.Span[i] = selector(source.Span[i]);
        }

        return result;
    }

    public static Memory<TResult> AsType<TTaget, TResult>(this Memory<TTaget> source)
        where TResult : class
    {
        if (source.IsEmpty)
        {
            return Memory<TResult>.Empty;
        }

        var result = new TResult[source.Length].AsMemory();
        var index = 0;

        for (var i = 0; i < source.Length; i++)
        {
            var item = source.Span[i] as TResult;

            if (item is null)
            {
                continue;
            }

            result.Span[index++] = item;
        }

        return result.Slice(0, index);
    }

    public static Memory<T> Reverse<T>(this ReadOnlyMemory<T> source)
    {
        if (source.IsEmpty)
        {
            return Memory<T>.Empty;
        }

        var result = new T[source.Length].AsMemory();

        for (var i = 0; i < source.Length; i++)
        {
            result.Span[i] = source.Span[source.Length - i - 1];
        }

        return result;
    }

    public static Memory<T> Reverse<T>(this Memory<T> source)
    {
        if (source.IsEmpty)
        {
            return Memory<T>.Empty;
        }

        source.Span.Reverse();

        return source;
    }

    public static bool All<T>(this Span<T> source, Func<T, bool> predicate)
    {
        foreach (var t in source)
        {
            if (!predicate.Invoke(t))
            {
                return false;
            }
        }

        return true;
    }

    public static bool Any<T>(this Span<T> source, Func<T, bool> predicate)
    {
        if (source.IsEmpty)
        {
            return false;
        }

        foreach (var t in source)
        {
            if (predicate.Invoke(t))
            {
                return true;
            }
        }

        return false;
    }

    public static Span<int> SelectIndexOf(this Span<string> source, ReadOnlySpan<char> target)
    {
        if (source.IsEmpty)
        {
            return Span<int>.Empty;
        }

        var result = new int[source.Length].AsSpan();

        for (var i = 0; i < source.Length; i++)
        {
            result[i] = target.IndexOf(source[i]);
        }

        return result;
    }

    public static Span<int> SelectIndexOf<T>(this Span<T> source, ReadOnlySpan<T> target)
    {
        if (source.IsEmpty)
        {
            return Span<int>.Empty;
        }

        var result = new int[source.Length].AsSpan();

        for (var i = 0; i < source.Length; i++)
        {
            result[i] = target.IndexOf(source[i]);
        }

        return result;
    }

    public static Span<int> SelectIndexOf<T>(this Span<T> source, Span<T> target)
    {
        if (source.IsEmpty)
        {
            return Span<int>.Empty;
        }

        var result = new int[source.Length].AsSpan();

        for (var i = 0; i < source.Length; i++)
        {
            result[i] = target.IndexOf(source[i]);
        }

        return result;
    }

    public static Span<int> SelectIndexOf<TTaget, TResult>(
        this Span<TTaget> source,
        Span<TResult> target,
        Func<TTaget, TResult> selector
    )
    {
        if (source.IsEmpty)
        {
            return Span<int>.Empty;
        }

        var result = new int[source.Length].AsSpan();

        for (var i = 0; i < source.Length; i++)
        {
            result[i] = target.IndexOf(selector(source[i]));
        }

        return result;
    }

    public static Span<TResult> SelectAsSpan<TTaget, TResult>(
        this Span<TTaget> source,
        Func<TTaget, TResult> selector
    )
    {
        if (source.IsEmpty)
        {
            return Span<TResult>.Empty;
        }

        var result = new TResult[source.Length].AsSpan();

        for (var i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }

        return result;
    }

    public static Span<T> SelectMany<T>(this Span<IEnumerable<T>> source)
    {
        if (source.IsEmpty)
        {
            return Span<T>.Empty;
        }

        var arrays = source.SelectAsSpan(x => x.ToArray());
        var result = new T[arrays.Sum(x => x.Length)].AsSpan();

        var currentIndex = 0;

        foreach (var array in arrays)
        {
            array.CopyTo(result.Slice(currentIndex));
            currentIndex += array.Length;
        }

        return result;
    }

    public static Memory<T> SelectMany<T>(this Memory<IEnumerable<T>> source)
    {
        return source.IsEmpty
            ? Memory<T>.Empty
            : source.SelectAsSpan(x => x.ToArray().AsMemory()).SelectMany();
    }

    public static Memory<T> SelectMany<T>(this Memory<Memory<T>> source)
    {
        if (source.IsEmpty)
        {
            return Memory<T>.Empty;
        }

        var result = new T[source.Sum(x => x.Length)].AsMemory();

        var currentIndex = 0;

        foreach (var array in source.Span)
        {
            array.CopyTo(result.Slice(currentIndex));
            currentIndex += array.Length;
        }

        return result;
    }

    public static Memory<T> Distinct<T>(this Memory<T> source)
    {
        if (source.Length <= 1)
        {
            return source;
        }

        var result = new T[source.Length].AsMemory();
        var uniqueCount = 0;

        if (result.Span.Contains(default))
        {
            uniqueCount++;
        }

        for (var i = 1; i < source.Length; i++)
        {
            if (result.Span.Contains(source.Span[i]))
            {
                continue;
            }

            result.Span[uniqueCount] = source.Span[i];
            uniqueCount++;
        }

        return result.Slice(0, uniqueCount);
    }

    public static Memory<TResult> SelectAsMemory<TTaget, TResult>(
        this Span<TTaget> source,
        Func<TTaget, TResult> selector
    )
    {
        if (source.IsEmpty)
        {
            return Memory<TResult>.Empty;
        }

        var result = new TResult[source.Length].AsMemory();

        for (var i = 0; i < source.Length; i++)
        {
            result.Span[i] = selector(source[i]);
        }

        return result;
    }

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

    public static int Sum<T>(this Memory<T> source, Func<T, int> selector)
    {
        var sum = 0;

        foreach (var item in source.Span)
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

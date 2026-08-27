using System.Runtime.CompilerServices;

namespace Gaia.Helpers;

public static class ObjectExtension
{
    public static Memory<T> Combine<T>(this Memory<T> source,IEnumerable< Memory<T>> items)
    {
        Memory<T>[] array = [default,..items];
        array[0] = source;
        
        return array.Combine();
    }
    
    public static Memory<T> Combine<T>(this IEnumerable<Memory<T>> source)
    {
        var array = source.ToArray();
        
        return array.Combine();
    }
    
    public static Memory<T> Combine<T>(this Memory<T>[] source)
    {
        var array = source.ToArray();
        var result = new T[array.Sum(x=>x.Length)].AsMemory();
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

    public static Memory<TResult> Select<TTaget, TResult>(
        this ReadOnlyMemory<TTaget> span,
        Func<TTaget, TResult> selector
    )
    {
        var result = new TResult[span.Length].AsMemory();

        for (var i = 0; i < span.Length; i++)
        {
            result.Span[i] = selector(span.Span[i]);
        }

        return result;
    }

    public static Memory<TResult> Select<TTaget, TResult>(
        this Memory<TTaget> span,
        Func<TTaget, TResult> selector
    )
    {
        var result = new TResult[span.Length].AsMemory();

        for (var i = 0; i < span.Length; i++)
        {
            result.Span[i] = selector(span.Span[i]);
        }

        return result;
    }

    public static Memory<TResult> AsType<TTaget, TResult>(this Memory<TTaget> span)
        where TResult : class
    {
        var result = new TResult[span.Length].AsMemory();
        var index = 0;

        for (var i = 0; i < span.Length; i++)
        {
            var item = span.Span[i] as TResult;

            if (item is null)
            {
                continue;
            }

            result.Span[index++] = item;
        }

        return result.Slice(0, index);
    }

    public static Memory<T> Reverse<T>(this ReadOnlyMemory<T> span)
    {
        var result = new T[span.Length].AsMemory();

        for (var i = 0; i < span.Length; i++)
        {
            result.Span[i] = span.Span[span.Length - i - 1];
        }

        return result;
    }

    public static Memory<T> Reverse<T>(this Memory<T> source)
    {
        source.Span.Reverse();

        return source;
    }

    public static bool All<T>(this Span<T> span, Func<T, bool> predicate)
    {
        foreach (var t in span)
        {
            if (!predicate.Invoke(t))
            {
                return false;
            }
        }

        return true;
    }

    public static bool Any<T>(this Span<T> span, Func<T, bool> predicate)
    {
        foreach (var t in span)
        {
            if (predicate.Invoke(t))
            {
                return true;
            }
        }

        return false;
    }

    public static Span<int> SelectIndexOf(this Span<string> span, ReadOnlySpan<char> source)
    {
        var result = new int[span.Length].AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            result[i] = source.IndexOf(span[i]);
        }

        return result;
    }

    public static Span<int> SelectIndexOf<TTaget>(
        this Span<TTaget> span,
        ReadOnlySpan<TTaget> source
    )
    {
        var result = new int[span.Length].AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            result[i] = source.IndexOf(span[i]);
        }

        return result;
    }

    public static Span<int> SelectIndexOf<TTaget>(this Span<TTaget> span, Span<TTaget> source)
    {
        var result = new int[span.Length].AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            result[i] = source.IndexOf(span[i]);
        }

        return result;
    }

    public static Span<int> SelectIndexOf<TTaget, TResult>(
        this Span<TTaget> span,
        Span<TResult> source,
        Func<TTaget, TResult> selector
    )
    {
        var result = new int[span.Length].AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            result[i] = source.IndexOf(selector(span[i]));
        }

        return result;
    }

    public static Span<TResult> Select<TTaget, TResult>(
        this Span<TTaget> span,
        Func<TTaget, TResult> selector
    )
    {
        var result = new TResult[span.Length].AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            result[i] = selector(span[i]);
        }

        return result;
    }

    public static Memory<TResult> SelectAsMemory<TTaget, TResult>(
        this Span<TTaget> span,
        Func<TTaget, TResult> selector
    )
    {
        var result = new TResult[span.Length].AsMemory();

        for (var i = 0; i < span.Length; i++)
        {
            result.Span[i] = selector(span[i]);
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

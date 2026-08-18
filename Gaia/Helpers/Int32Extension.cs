namespace Gaia.Helpers;

public static class Int32Extension
{
    public static int Max(this Span<int> span)
    {
        if (span.Length == 0)
        {
            throw new ArgumentException("Span is empty");
        }

        var result = int.MinValue;

        foreach (var item in span)
        {
            if (item > result)
            {
                result = item;
            }
        }

        return result;
    }

    public static int Min(this Span<int> span)
    {
        if (span.Length == 0)
        {
            throw new ArgumentException("Span is empty");
        }

        var result = int.MaxValue;

        foreach (var item in span)
        {
            if (item < result)
            {
                result = item;
            }
        }

        return result;
    }
}

using System.Diagnostics.CodeAnalysis;

namespace Gaia.Helpers;

public static class StringExtension
{
    public static string RemoveDuplicateSpaces(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var span = input.AsSpan();
        var finalLength = 0;
        var lastWasSpace = false;

        for (var i = 0; i < span.Length; i++)
        {
            var isSpace = span[i] == ' ';

            if (!isSpace || !lastWasSpace)
            {
                finalLength++;
            }

            lastWasSpace = isSpace;
        }

        if (finalLength == span.Length)
        {
            return input;
        }

        return string.Create(
            finalLength,
            input,
            (dest, src) =>
            {
                var destIndex = 0;
                var innerLastWasSpace = false;

                for (var i = 0; i < src.Length; i++)
                {
                    var isSpace = src[i] == ' ';
                    if (!isSpace || !innerLastWasSpace)
                    {
                        dest[destIndex++] = src[i];
                    }
                    innerLastWasSpace = isSpace;
                }
            }
        );
    }

    public static string GetLengthWithSpace(this string str, ushort length)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return new string(' ', length);
        }

        if (str.Length >= length)
        {
            return str;
        }

        return $"{str}{new string(' ', length - str.Length - 1)}";
    }

    public static Guid ToGuid(this string id)
    {
        return Guid.Parse(id);
    }

    public static string JoinString(this IEnumerable<string> enumerable, string separator)
    {
        return string.Join(separator, enumerable);
    }

    public static DirectoryInfo ToDir(this string path)
    {
        return new(path);
    }

    public static bool IsEmail(this string str)
    {
        return StringHelper.EmailRegex.IsMatch(str);
    }

    public static bool IsLink(this string str)
    {
        return str.StartsWith("http://") || str.StartsWith("https://");
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static Uri ToUri(this string str)
    {
        return new(str);
    }
}

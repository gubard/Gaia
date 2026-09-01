using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Gaia.Models;

namespace Gaia.Helpers;

public static class StringExtension
{
    public static Memory<ValidationError> ValidatePropertyPhoneNumber(
        this string str,
        string propertyName
    )
    {
        var normalized = str.NormalizePhoneNumber();

        if (normalized.IsNullOrWhiteSpace())
        {
            return new ValidationError[] { new PropertyEmptyValidationError(propertyName) };
        }

        if (normalized.Length < 10)
        {
            return new ValidationError[]
            {
                new PropertyMinSizeValidationError(propertyName, (uint)normalized.Length, 10),
            };
        }

        if (normalized[0] == '0')
        {
            return new ValidationError[]
            {
                new PropertyStartWithValidationError(propertyName, "0"),
            };
        }

        return Memory<ValidationError>.Empty;
    }

    public static string NormalizePhoneNumber(this string str)
    {
        var result = new StringBuilder();

        foreach (var c in str)
        {
            if (char.IsDigit(c))
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    public static string ConsoleWriteLine(this string str)
    {
        Console.WriteLine(str);

        return str;
    }

    public static TEnum ParseEnum<TEnum>(this string str)
        where TEnum : struct
    {
        return Enum.Parse<TEnum>(str);
    }

    public static string ReplaceWholeWords(this string text, Dictionary<string, string> map)
    {
        if (string.IsNullOrEmpty(text) || map.Count == 0)
        {
            return text;
        }

        var span = text.AsSpan();
        var keys = new string[map.Count];
        map.Keys.CopyTo(keys, 0);
        var searcher = SearchValues.Create(keys, StringComparison.Ordinal);
        var sb = new StringBuilder(text.Length);
        var lastPos = 0;

        while (lastPos < span.Length)
        {
            var matchIndex = span.Slice(lastPos).IndexOfAny(searcher);

            if (matchIndex == -1)
            {
                break;
            }

            var absoluteMatchIndex = lastPos + matchIndex;

            foreach (var (target, replacement) in map)
            {
                if (!span.Slice(absoluteMatchIndex).StartsWith(target.AsSpan()))
                {
                    continue;
                }

                var matchEnd = absoluteMatchIndex + target.Length;

                var isLeftBoundary =
                    absoluteMatchIndex == 0 || !span[absoluteMatchIndex - 1].IsWordChar();

                var isRightBoundary = matchEnd == span.Length || !span[matchEnd].IsWordChar();

                if (isLeftBoundary && isRightBoundary)
                {
                    sb.Append(span.Slice(lastPos, absoluteMatchIndex - lastPos));
                    sb.Append(replacement);
                    lastPos = matchEnd;
                }
                else
                {
                    sb.Append(span.Slice(lastPos, absoluteMatchIndex + 1 - lastPos));
                    lastPos = absoluteMatchIndex + 1;
                }

                break;
            }
        }

        sb.Append(span.Slice(lastPos));

        return sb.ToString();
    }

    public static string ReplaceWholeWord(this string text, string target, string replacement)
    {
        if (text.IsNullOrEmpty() || target.IsNullOrEmpty())
        {
            return text;
        }

        var span = text.AsSpan();
        var targetSpan = target.AsSpan();

        var index = span.IndexOf(targetSpan);

        if (index == -1)
        {
            return text;
        }

        var sb = new StringBuilder(text.Length);
        var lastIndex = 0;

        while (index != -1)
        {
            var matchEnd = index + targetSpan.Length;
            var isLeftBoundary = index == 0 || !span[index - 1].IsWordChar();
            var isRightBoundary = matchEnd == span.Length || !span[matchEnd].IsWordChar();

            if (isLeftBoundary && isRightBoundary)
            {
                sb.Append(span.Slice(lastIndex, index - lastIndex));
                sb.Append(replacement);
                lastIndex = matchEnd;
                index = matchEnd <= span.Length ? span.Slice(matchEnd).IndexOf(targetSpan) : -1;

                if (index != -1)
                {
                    index += lastIndex;
                }
            }
            else
            {
                var nextStart = index + 1;

                if (nextStart >= span.Length)
                {
                    break;
                }

                var nextIndex = span.Slice(nextStart).IndexOf(targetSpan);
                index = nextIndex == -1 ? -1 : nextStart + nextIndex;
            }
        }

        sb.Append(span.Slice(lastIndex));

        return sb.ToString();
    }

    public static string ToUpperFirstChar(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        return string.Create(
            str.Length,
            str,
            (span, state) =>
            {
                span[0] = char.ToUpperInvariant(state[0]);
                state.AsSpan(1).CopyTo(span.Slice(1));
            }
        );
    }

    public static string ToLowerFirstChar(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        return string.Create(
            str.Length,
            str,
            (span, state) =>
            {
                span[0] = char.ToLowerInvariant(state[0]);
                state.AsSpan(1).CopyTo(span.Slice(1));
            }
        );
    }

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

    public static string JoinString(this Span<string> enumerable, string separator)
    {
        return string.Join(separator, enumerable);
    }

    public static string JoinString(this Memory<string> enumerable, string separator)
    {
        return string.Join(separator, enumerable.Span);
    }

    public static string JoinString(this ReadOnlyMemory<string> enumerable, string separator)
    {
        return string.Join(separator, enumerable.Span);
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

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static Uri ToUri(this string str)
    {
        return new(str);
    }
}

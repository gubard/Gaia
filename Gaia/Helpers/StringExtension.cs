using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gaia.Helpers;

public static class StringExtension
{
    public static string ReplaceWholeWords(string text, Span<KeyValuePair<string, string>> targets)
    {
        if (text.IsNullOrEmpty())
        {
            return text;
        }

        var span = text.AsSpan();
        var targetSpans = targets.Select(x => x.Key);
        var indexs = targetSpans.SelectIndexOf(span);

        if (indexs.All(x => x == -1))
        {
            return text;
        }

        var sb = new StringBuilder(text.Length);
        var lastIndex = 0;

        while (indexs.All(x => x != -1))
        {
            for (var i = 0; i < indexs.Length; i++)
            {
                var targetSpan = targetSpans[i];
                var replacement = targets[i].Value.AsSpan();
                var matchEnd = indexs[i] + targetSpan.Length;
                var isLeftBoundary = indexs[i] == 0 || !span[indexs[i] - 1].IsWordChar();
                var isRightBoundary = matchEnd == span.Length || !span[matchEnd].IsWordChar();

                if (isLeftBoundary && isRightBoundary)
                {
                    sb.Append(span.Slice(lastIndex, indexs[i] - lastIndex));
                    sb.Append(replacement);
                    lastIndex = matchEnd;

                    indexs[i] =
                        matchEnd <= span.Length ? span.Slice(matchEnd).IndexOf(targetSpan) : -1;

                    if (indexs[i] != -1)
                    {
                        indexs[i] += lastIndex;
                    }
                }
                else
                {
                    var nextStart = indexs[i] + 1;

                    if (nextStart >= span.Length)
                    {
                        break;
                    }

                    var nextIndex = span.Slice(nextStart).IndexOf(targetSpan);
                    indexs[i] = nextIndex == -1 ? -1 : nextStart + nextIndex;
                }
            }
        }

        sb.Append(span.Slice(lastIndex));

        return sb.ToString();
    }

    public static string ReplaceWholeWord(string text, string target, string replacement)
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

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static Uri ToUri(this string str)
    {
        return new(str);
    }
}

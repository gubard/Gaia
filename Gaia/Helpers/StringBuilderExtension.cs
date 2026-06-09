using System.Text;

namespace Gaia.Helpers;

public static class StringBuilderExtension
{
    public static void Duplicate(this StringBuilder builder, string str, ulong count)
    {
        if (count == 0)
        {
            return;
        }

        for (var i = 0ul; i < count; i++)
        {
            builder.Append(str);
        }
    }

    public static string ToTitle(this string str)
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
                state.AsSpan(1).ToLowerInvariant(span.Slice(1));
            }
        );
    }
}

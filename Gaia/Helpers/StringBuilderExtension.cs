using System.Text;

namespace Gaia.Helpers;

public static class StringBuilderExtension
{
    public static void Duplicate(this StringBuilder builder, string str,
        ulong count)
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
}
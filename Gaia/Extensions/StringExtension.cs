using System.Diagnostics.CodeAnalysis;

namespace Gaia.Extensions;

public static class StringExtension
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}
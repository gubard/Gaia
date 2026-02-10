using System.Diagnostics.CodeAnalysis;

namespace Gaia.Helpers;

public static class StringExtension
{
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

namespace Gaia.Helpers;

public static class StringExtension
{
    public static bool IsEmail(this string str)
    {
        return StringHelper.EmailRegex.IsMatch(str);
    }
}
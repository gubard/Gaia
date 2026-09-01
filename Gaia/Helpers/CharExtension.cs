namespace Gaia.Helpers;

public static class CharExtension
{
    public static bool IsWordChar(this char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }

    public static bool IsDigit(this char c)
    {
        return char.IsDigit(c);
    }
}

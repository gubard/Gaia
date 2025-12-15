namespace Gaia.Services;

public static class StringExtension
{
    public static TEnum ParseEnum<TEnum>(this string str)
        where TEnum : struct
    {
        return Enum.Parse<TEnum>(str);
    }
}

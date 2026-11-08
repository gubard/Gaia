namespace Gaia.Helpers;

public static class GaiaHelper
{
    public static DateTime? Min(DateTime? x, DateTime? y)
    {
        if (x is null)
        {
            return y;
        }

        if (y is null)
        {
            return x;
        }

        return x > y ? y : x;
    }
}

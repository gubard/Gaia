using System.Globalization;

namespace Gaia.Helpers;

public static class CultureHelper
{
    public static readonly CultureInfo Ukranian = new("uk-UA");

    public static void SetAppCulture(CultureInfo culture)
    {
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}

using Gaia.Models;

namespace Gaia.Helpers;

public static class OsHelper
{
    public static readonly Os OsType;
    public static readonly bool IsMobile;
    public static readonly bool IsDesktop;

    static OsHelper()
    {
        if (OperatingSystem.IsWindows())
        {
            OsType = Os.Windows;
            IsDesktop = true;
        }
        else if (OperatingSystem.IsMacOS())
        {
            OsType = Os.MacOs;
            IsDesktop = true;
        }
        else if (OperatingSystem.IsLinux())
        {
            OsType = Os.Linux;
            IsDesktop = true;
        }
        else if (OperatingSystem.IsAndroid())
        {
            OsType = Os.Android;
            IsMobile = true;
        }
        else if (OperatingSystem.IsBrowser())
        {
            OsType = Os.Browser;
        }
        else if (OperatingSystem.IsFreeBSD())
        {
            OsType = Os.FreeBsd;
            IsDesktop = true;
        }
        else if (OperatingSystem.IsIOS())
        {
            OsType = Os.iOS;
            IsMobile = true;
        }
        else if (OperatingSystem.IsMacCatalyst())
        {
            OsType = Os.MacCatalyst;
            IsDesktop = true;
        }
        else if (OperatingSystem.IsTvOS())
        {
            OsType = Os.TvOs;
        }
        else if (OperatingSystem.IsWatchOS())
        {
            OsType = Os.WatchOs;
        }
        else if (OperatingSystem.IsWasi())
        {
            OsType = Os.Wasi;
        }
    }
}

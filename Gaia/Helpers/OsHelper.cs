using Gaia.Models;

namespace Gaia.Helpers;

public static class OsHelper
{
    public static readonly Os OsType;

    static OsHelper()
    {
        if (OperatingSystem.IsWindows())
        {
            OsType = Os.Windows;
        }
        else if (OperatingSystem.IsMacOS())
        {
            OsType = Os.MacOs;
        }
        else if (OperatingSystem.IsLinux())
        {
            OsType = Os.Linux;
        }
        else if (OperatingSystem.IsAndroid())
        {
            OsType = Os.Android;
        }
        else if (OperatingSystem.IsBrowser())
        {
            OsType = Os.Browser;
        }
        else if (OperatingSystem.IsFreeBSD())
        {
            OsType = Os.FreeBsd;
        }
        else if (OperatingSystem.IsIOS())
        {
            OsType = Os.Ios;
        }
        else if (OperatingSystem.IsMacCatalyst())
        {
            OsType = Os.MacCatalyst;
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

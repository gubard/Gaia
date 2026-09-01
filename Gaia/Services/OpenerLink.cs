using System.Diagnostics;
using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IOpenerLink
{
    ConfiguredValueTaskAwaitable OpenLinkAsync(Uri link, CancellationToken ct);
}

public sealed class DesktopOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable OpenLinkAsync(Uri link, CancellationToken ct)
    {
        switch (OsHelper.OsType)
        {
            case Os.Windows:
                var url = link.AbsoluteUri.Replace("&", "^&");

                var info = new ProcessStartInfo(url) { UseShellExecute = true };

                Process.Start(info);

                return TaskHelper.ConfiguredCompletedTask;
            case Os.MacOs:
                Process.Start("open", link.AbsoluteUri);

                return TaskHelper.ConfiguredCompletedTask;
            case Os.Linux:
                Process.Start("xdg-open", link.AbsoluteUri);

                return TaskHelper.ConfiguredCompletedTask;
            case Os.Android:
            case Os.Browser:
            case Os.FreeBsd:
            case Os.iOS:
            case Os.MacCatalyst:
            case Os.TvOs:
            case Os.WatchOs:
            case Os.Wasi:
            default:
                throw new NotSupportedException();
        }
    }
}

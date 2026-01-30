using System.Runtime.CompilerServices;

namespace Gaia.Services;

public interface IOpenerLink
{
    ConfiguredValueTaskAwaitable OpenLinkAsync(Uri link, CancellationToken ct);
}

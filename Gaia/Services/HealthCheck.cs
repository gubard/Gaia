using System.Runtime.CompilerServices;

namespace Gaia.Services;

public interface IHealthCheck
{
    ConfiguredValueTaskAwaitable<bool> HealthCheckAsync(CancellationToken ct);
    bool HealthCheck();
}

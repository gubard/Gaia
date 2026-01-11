using System.Runtime.CompilerServices;

namespace Gaia.Services;

public interface IHealthCheck
{
    ConfiguredValueTaskAwaitable<IValidationErrors> HealthCheckAsync(CancellationToken ct);
}

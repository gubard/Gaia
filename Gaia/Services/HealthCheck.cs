using System.Runtime.CompilerServices;
using Gaia.Models;

namespace Gaia.Services;

public interface IHealthCheck
{
    ConfiguredValueTaskAwaitable<Result> HealthCheckAsync(CancellationToken ct);
}

using System.Runtime.CompilerServices;

namespace Gaia.Services;

public interface ICache<in TPostRequest, in TGetResponse>
{
    ConfiguredValueTaskAwaitable UpdateAsync(TPostRequest source, CancellationToken ct);
    ConfiguredValueTaskAwaitable UpdateAsync(TGetResponse source, CancellationToken ct);
}

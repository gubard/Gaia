using System.Runtime.CompilerServices;
using Gaia.Helpers;

namespace Gaia.Services;

public interface ICache<in TPostRequest, in TGetResponse>
{
    ConfiguredValueTaskAwaitable UpdateAsync(TPostRequest source, CancellationToken ct);
    ConfiguredValueTaskAwaitable UpdateAsync(TGetResponse source, CancellationToken ct);
}

public abstract class EmptyCache<TPostRequest, TGetResponse> : ICache<TPostRequest, TGetResponse>
{
    public ConfiguredValueTaskAwaitable UpdateAsync(TPostRequest source, CancellationToken ct)
    {
        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(TGetResponse source, CancellationToken ct)
    {
        return TaskHelper.ConfiguredCompletedTask;
    }
}

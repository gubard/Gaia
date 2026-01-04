using System.Runtime.CompilerServices;

namespace Gaia.Helpers;

public static class TaskHelper
{
    public static readonly ConfiguredValueTaskAwaitable ConfiguredCompletedTask =
        ValueTask.CompletedTask.ConfigureAwait(false);

    public static ConfiguredValueTaskAwaitable<T> FromResult<T>(T value)
    {
        return ValueTask.FromResult(value).ConfigureAwait(false);
    }
}

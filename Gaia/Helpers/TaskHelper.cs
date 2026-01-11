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

    public static ConfiguredValueTaskAwaitable WhenAllAsync(
        params ConfiguredValueTaskAwaitable[] tasks
    )
    {
        return WhenAllCore(tasks).ConfigureAwait(false);
    }

    private static async ValueTask WhenAllCore(ConfiguredValueTaskAwaitable[] tasks)
    {
        foreach (var task in tasks)
        {
            await task;
        }
    }
}

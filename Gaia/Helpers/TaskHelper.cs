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
        ConfiguredValueTaskAwaitable[] tasks,
        CancellationToken ct
    )
    {
        return WhenAllCore(tasks, ct).ConfigureAwait(false);
    }

    private static async ValueTask WhenAllCore(
        ConfiguredValueTaskAwaitable[] tasks,
        CancellationToken ct
    )
    {
        foreach (var task in tasks)
        {
            ct.ThrowIfCancellationRequested();

            await task;
        }
    }

    public static ConfiguredValueTaskAwaitable<TResult[]> WhenAllAsync<TResult>(
        ValueTask<TResult>[] tasks,
        CancellationToken ct
    )
    {
        if (tasks.Length == 0)
        {
            return FromResult(Array.Empty<TResult>());
        }

        return WhenAllCore(tasks, ct).ConfigureAwait(false);
    }

    private static async ValueTask<TResult[]> WhenAllCore<TResult>(
        ValueTask<TResult>[] tasks,
        CancellationToken ct
    )
    {
        var result = new TResult[tasks.Length];

        for (var index = 0; index < tasks.Length; index++)
        {
            ct.ThrowIfCancellationRequested();
            var task = tasks[index];
            result[index] = await task;
        }

        return result;
    }
}

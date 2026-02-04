using System.Runtime.CompilerServices;
using Gaia.Services;

namespace Gaia.Helpers;

public static class ObjectStorageExtension
{
    public static ConfiguredValueTaskAwaitable<T> LoadAsync<T>(
        this IObjectStorage storage,
        CancellationToken ct
    )
        where T : IObjectStorageValue, new()
    {
        return storage.LoadAsync<T>(T.GetObjectStorageKey(), ct);
    }

    public static ConfiguredValueTaskAwaitable<T> LoadAsync<T>(
        this IObjectStorage storage,
        Guid id,
        CancellationToken ct
    )
        where T : IObjectStorageValue, new()
    {
        return storage.LoadAsync<T>(T.GetObjectStorageKey(id), ct);
    }

    public static ConfiguredValueTaskAwaitable SaveAsync<T>(
        this IObjectStorage storage,
        T obj,
        CancellationToken ct
    )
        where T : IObjectStorageValue
    {
        return storage.SaveAsync(T.GetObjectStorageKey(), obj, ct);
    }

    public static ConfiguredValueTaskAwaitable SaveAsync<T>(
        this IObjectStorage storage,
        T obj,
        Guid id,
        CancellationToken ct
    )
        where T : IObjectStorageValue
    {
        return storage.SaveAsync(T.GetObjectStorageKey(id), obj, ct);
    }
}

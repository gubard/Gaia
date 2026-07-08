using System.Runtime.CompilerServices;
using Gaia.Helpers;

namespace Gaia.Services;

public interface IObjectStorage
{
    ConfiguredValueTaskAwaitable<T> LoadAsync<T>(string key, CancellationToken ct)
        where T : IStaticFactory<T>;

    ConfiguredValueTaskAwaitable SaveAsync(string key, object obj, CancellationToken ct);
}

public sealed class MemoryObjectStorage : IObjectStorage
{
    private readonly Dictionary<string, object> _storage = new();

    public ConfiguredValueTaskAwaitable<T> LoadAsync<T>(string key, CancellationToken ct)
        where T : IStaticFactory<T>
    {
        if (_storage.TryGetValue(key, out var value))
        {
            return TaskHelper.FromResult((T)value);
        }

        return TaskHelper.FromResult(T.Create());
    }

    public ConfiguredValueTaskAwaitable SaveAsync(string key, object obj, CancellationToken ct)
    {
        _storage[key] = obj;

        return TaskHelper.ConfiguredCompletedTask;
    }
}

public sealed class FileObjectStorage : IObjectStorage
{
    public FileObjectStorage(DirectoryInfo directory, ISerializer serializer)
    {
        _directory = directory;
        _serializer = serializer;
    }

    public ConfiguredValueTaskAwaitable<T> LoadAsync<T>(string key, CancellationToken ct)
        where T : IStaticFactory<T>
    {
        return LoadCore<T>(key, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable SaveAsync(string key, object obj, CancellationToken ct)
    {
        return SaveCore(key, obj, ct).ConfigureAwait(false);
    }

    private readonly DirectoryInfo _directory;
    private readonly ISerializer _serializer;

    private async ValueTask SaveCore(string key, object obj, CancellationToken ct)
    {
        var file = _directory.ToFile($"{key}.{_serializer.FileExtension}");

        if (file.Exists)
        {
            file.Delete();
        }

        await using var stream = file.Create();
        await _serializer.SerializeAsync(stream, obj, ct);
    }

    private async ValueTask<T> LoadCore<T>(string key, CancellationToken ct)
        where T : IStaticFactory<T>
    {
        var file = _directory.ToFile($"{key}.{_serializer.FileExtension}");

        if (!file.Exists)
        {
            return T.Create();
        }

        await using var stream = file.OpenRead();
        var value = await SafeDeserializeAsync<T>(stream, ct);

        return value ?? T.Create();
    }

    private async ValueTask<T?> SafeDeserializeAsync<T>(Stream stream, CancellationToken ct)
    {
        try
        {
            return await _serializer.DeserializeAsync<T>(stream, ct);
        }
        catch
        {
            return default;
        }
    }
}

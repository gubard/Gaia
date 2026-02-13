using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Gaia.Services;

public interface ISerializer
{
    string FileExtension { get; }

    ConfiguredValueTaskAwaitable<T?> DeserializeAsync<T>(Stream stream, CancellationToken ct);
    ConfiguredValueTaskAwaitable SerializeAsync(Stream stream, object obj, CancellationToken ct);
}

public sealed class JsonSerializer : ISerializer
{
    public JsonSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    public string FileExtension => "json";

    public ConfiguredValueTaskAwaitable<T?> DeserializeAsync<T>(Stream stream, CancellationToken ct)
    {
        return DeserializeCore<T>(stream, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable SerializeAsync(
        Stream stream,
        object obj,
        CancellationToken ct
    )
    {
        return SerializeCore(stream, obj, ct).ConfigureAwait(false);
    }

    private readonly JsonSerializerOptions _options;

    private ValueTask<T?> DeserializeCore<T>(Stream stream, CancellationToken ct)
    {
        return System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, _options, ct);
    }

    private async ValueTask SerializeCore(Stream stream, object obj, CancellationToken ct)
    {
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, _options, ct);
    }
}

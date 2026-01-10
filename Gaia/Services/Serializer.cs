using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Gaia.Services;

public interface ISerializer
{
    string FileExtension { get; }

    ConfiguredValueTaskAwaitable<T?> DeserializeAsync<T>(Stream stream, CancellationToken ct);
    ConfiguredValueTaskAwaitable SerializeAsync(Stream stream, object obj, CancellationToken ct);
    void Serialize(Stream stream, object obj);
}

public class JsonSerializer : ISerializer
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

    public void Serialize(Stream stream, object obj)
    {
        System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
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

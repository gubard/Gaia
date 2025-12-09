using System.Text.Json;

namespace Gaia.Helpers;

public static class HttpContentExtension
{
    public static TValue? ReadFromJson<TValue>(this HttpContent content,
        JsonSerializerOptions jsonSerializerOptions)
    {
        using var stream = content.ReadAsStream();

        return JsonSerializer.Deserialize<TValue>(stream,
            jsonSerializerOptions);
    }
}
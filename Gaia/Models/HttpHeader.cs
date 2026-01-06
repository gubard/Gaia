namespace Gaia.Models;

public record struct HttpHeader
{
    public const string Authorization = "Authorization";
    public const string Bearer = "Bearer";
    public const string TimeZoneOffset = "X-Time-Zone-Offset";
    public const string UserId = "X-User-Id";
    public const string IdempotentId = "X-Idempotent-Id";

    public HttpHeader(string name, params Span<string> values)
    {
        Name = name;
        Values = values.ToArray();
    }

    public string Name { get; }
    public ReadOnlyMemory<string> Values { get; }
}

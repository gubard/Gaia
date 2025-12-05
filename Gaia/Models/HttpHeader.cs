namespace Gaia.Models;

public record struct HttpHeader
{
    public const string Authorization = "Authorization";
    public const string Bearer = "Bearer";

    public HttpHeader(string name, params Span<string> values)
    {
        Name = name;
        Values = values.ToArray();
    }

    public string Name { get; }
    public ReadOnlyMemory<string> Values { get; }
}
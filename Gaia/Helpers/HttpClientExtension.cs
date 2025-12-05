using Gaia.Models;

namespace Gaia.Helpers;

public static class HttpClientExtension
{
    public static HttpClient AddHeaders(this HttpClient httpClient, ReadOnlySpan<HttpHeader> headers)
    {
        httpClient.DefaultRequestHeaders.Clear();

        foreach (var header in headers)
        {
            httpClient.DefaultRequestHeaders.Add(header.Name, header.Values.ToArray());
        }

        return httpClient;
    }
}
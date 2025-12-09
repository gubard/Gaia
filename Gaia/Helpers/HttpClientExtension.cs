using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using Gaia.Models;

namespace Gaia.Helpers;

public static class HttpClientExtension
{
    extension(HttpClient httpClient)
    {
        public HttpClient AddHeaders(ReadOnlySpan<HttpHeader> headers)
        {
            httpClient.DefaultRequestHeaders.Clear();

            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Name,
                    header.Values.ToArray());
            }

            return httpClient;
        }

        public HttpResponseMessage PostAsJson<TValue>(
            [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
            TValue value, JsonSerializerOptions options)
        {
            var content = JsonContent.Create(value, null, options);

            return httpClient.Post(requestUri, content);
        }

        public HttpResponseMessage Post(
            [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
            HttpContent content)
        {
            return httpClient.Post(CreateUri(requestUri), content);
        }

        public HttpResponseMessage Post(Uri requestUri, HttpContent content)
        {
            var request =
                httpClient.CreateRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;

            return httpClient.Send(request);
        }
    }

    private static Uri CreateUri(string uri)
    {
        return new(uri, UriKind.RelativeOrAbsolute);
    }

    private static HttpRequestMessage CreateRequestMessage(
        this HttpClient httpClient, HttpMethod method, Uri uri)
    {
        return new(method, uri)
        {
            Version = httpClient.DefaultRequestVersion,
            VersionPolicy = httpClient.DefaultVersionPolicy,
        };
    }
}
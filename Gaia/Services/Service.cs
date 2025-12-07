using System.Net.Http.Json;
using System.Text.Json;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse> where TGetResponse : IValidationErrors, new() where TPostResponse : IValidationErrors, new()
{
    ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct);
    ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct);
}

public interface IHttpService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse> : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse> where TGetResponse : IValidationErrors, new() where TPostResponse : IValidationErrors, new();

public abstract class HttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse> : IHttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse> where TGetResponse : IValidationErrors, new() where TPostResponse : IValidationErrors, new()
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ITryPolicyService _tryPolicyService;
    private readonly IFactory<Memory<HttpHeader>> _headersFactory;

    protected HttpService(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions, ITryPolicyService tryPolicyService, IFactory<Memory<HttpHeader>> headersFactory)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _tryPolicyService = tryPolicyService;
        _headersFactory = headersFactory;
    }

    public ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct)
    {
        return _tryPolicyService.TryAsync(async () =>
        {
            var headers = _headersFactory.Create();
            using var httpResponse = await _httpClient.AddHeaders(headers.Span).PostAsJsonAsync(RouteHelper.Get, request, _jsonSerializerOptions, ct);
            var response = await httpResponse.Content.ReadFromJsonAsync<TGetResponse>(_jsonSerializerOptions, ct);

            return response.ThrowIfNull();
        });
    }

    public ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct)
    {
        return _tryPolicyService.TryAsync(async () =>
        {
            var headers = _headersFactory.Create();
            using var httpResponse = await _httpClient.AddHeaders(headers.Span).PostAsJsonAsync(RouteHelper.Post, request, _jsonSerializerOptions, ct);
            var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(_jsonSerializerOptions, ct);

            return response.ThrowIfNull();
        });
    }
}
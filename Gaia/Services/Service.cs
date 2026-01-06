using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, new()
    where TPostResponse : IValidationErrors, new()
{
    ConfiguredValueTaskAwaitable<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct);

    ConfiguredValueTaskAwaitable<TPostResponse> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    );

    TPostResponse Post(Guid idempotentId, TPostRequest request);
    TGetResponse Get(TGetRequest request);
}

public abstract class HttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, new()
    where TPostResponse : IValidationErrors, new()
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ITryPolicyService _tryPolicyService;
    private readonly IFactory<Memory<HttpHeader>> _headersFactory;

    protected HttpService(
        HttpClient httpClient,
        JsonSerializerOptions jsonSerializerOptions,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory
    )
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _tryPolicyService = tryPolicyService;
        _headersFactory = headersFactory;
    }

    public ConfiguredValueTaskAwaitable<TGetResponse> GetAsync(
        TGetRequest request,
        CancellationToken ct
    )
    {
        return _tryPolicyService.TryAsync(async () =>
        {
            var headers = _headersFactory.Create();

            using var httpResponse = await _httpClient
                .SetHeaders(headers.Span)
                .PostAsJsonAsync(RouteHelper.Get, request, _jsonSerializerOptions, ct);

            var response = await httpResponse.Content.ReadFromJsonAsync<TGetResponse>(
                _jsonSerializerOptions,
                ct
            );

            return response.ThrowIfNull();
        });
    }

    public ConfiguredValueTaskAwaitable<TPostResponse> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        return _tryPolicyService.TryAsync(async () =>
        {
            var headers = _headersFactory.Create();

            using var httpResponse = await _httpClient
                .SetHeaders(headers.Span)
                .AddHeader(new(HttpHeader.IdempotentId, idempotentId.ToString()))
                .PostAsJsonAsync(RouteHelper.Post, request, _jsonSerializerOptions, ct);

            var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(
                _jsonSerializerOptions,
                ct
            );

            return response.ThrowIfNull();
        });
    }

    public TPostResponse Post(Guid idempotentId, TPostRequest request)
    {
        return _tryPolicyService.Try(() =>
        {
            var headers = _headersFactory.Create();

            using var httpResponse = _httpClient
                .SetHeaders(headers.Span)
                .AddHeader(new(HttpHeader.IdempotentId, idempotentId.ToString()))
                .PostAsJson(RouteHelper.Post, request, _jsonSerializerOptions);

            var response = httpResponse.Content.ReadFromJson<TPostResponse>(_jsonSerializerOptions);

            return response.ThrowIfNull();
        });
    }

    public TGetResponse Get(TGetRequest request)
    {
        return _tryPolicyService.Try(() =>
        {
            var headers = _headersFactory.Create();

            using var httpResponse = _httpClient
                .SetHeaders(headers.Span)
                .PostAsJson(RouteHelper.Get, request, _jsonSerializerOptions);

            var response = httpResponse.Content.ReadFromJson<TGetResponse>(_jsonSerializerOptions);

            return response.ThrowIfNull();
        });
    }
}

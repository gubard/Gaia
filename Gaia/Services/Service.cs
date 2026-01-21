using System.Net;
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
}

public interface IHttpService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>,
        IHealthCheck
    where TGetResponse : IValidationErrors, new()
    where TPostResponse : IValidationErrors, new();

public abstract class HttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    : IHttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, new()
    where TPostResponse : IValidationErrors, new()
    where TGetRequest : new()
{
    public ConfiguredValueTaskAwaitable<TGetResponse> GetAsync(
        TGetRequest request,
        CancellationToken ct
    )
    {
        return _tryPolicyService.TryAsync(() => GetRequestAsync(request, ct).ConfigureAwait(false));
    }

    public ConfiguredValueTaskAwaitable<TPostResponse> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        return _tryPolicyService.TryAsync(() =>
            PostRequestAsync(idempotentId, request, ct).ConfigureAwait(false)
        );
    }

    public ConfiguredValueTaskAwaitable<IValidationErrors> HealthCheckAsync(CancellationToken ct)
    {
        return HealthCheckCore(ct).ConfigureAwait(false);
    }

    public async ValueTask<IValidationErrors> HealthCheckCore(CancellationToken ct)
    {
        var response = await GetAsync(CreateHealthCheckGetRequest(), ct);

        return response;
    }

    protected HttpService(
        HttpClient httpClient,
        JsonSerializerOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory
    )
    {
        _httpClient = httpClient;
        _options = options;
        _tryPolicyService = tryPolicyService;
        _headersFactory = headersFactory;
    }

    protected abstract TGetRequest CreateHealthCheckGetRequest();

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly ITryPolicyService _tryPolicyService;
    private readonly IFactory<Memory<HttpHeader>> _headersFactory;

    private async ValueTask<TGetResponse> GetRequestAsync(TGetRequest request, CancellationToken ct)
    {
        var headers = _headersFactory.Create();

        using var httpResponse = await _httpClient
            .SetHeaders(headers.Span)
            .PostAsJsonAsync(RouteHelper.Get, request, _options, ct);

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            var result = new TGetResponse();
            result.ValidationErrors.Add(new UnauthorizedValidationError());

            return result;
        }

        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadFromJsonAsync<TGetResponse>(_options, ct);

        return response.ThrowIfNull();
    }

    private async ValueTask<TPostResponse> PostRequestAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        var headers = _headersFactory.Create();

        using var httpResponse = await _httpClient
            .SetHeaders(headers.Span)
            .AddHeader(new(HttpHeader.IdempotentId, idempotentId.ToString()))
            .PostAsJsonAsync(RouteHelper.Post, request, _options, ct);

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            var result = new TPostResponse();
            result.ValidationErrors.Add(new UnauthorizedValidationError());

            return result;
        }

        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(_options, ct);

        return response.ThrowIfNull();
    }
}

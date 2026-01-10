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
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly ITryPolicyService _tryPolicyService;
    private readonly IFactory<Memory<HttpHeader>> _headersFactory;

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

    public TPostResponse Post(Guid idempotentId, TPostRequest request)
    {
        return _tryPolicyService.Try(() =>
        {
            var headers = _headersFactory.Create();

            using var httpResponse = _httpClient
                .SetHeaders(headers.Span)
                .AddHeader(new(HttpHeader.IdempotentId, idempotentId.ToString()))
                .PostAsJson(RouteHelper.Post, request, _options);

            var response = httpResponse.Content.ReadFromJson<TPostResponse>(_options);

            return response.ThrowIfNull();
        });
    }

    public TGetResponse Get(TGetRequest request)
    {
        return _tryPolicyService.Try(() => GetRequest(request));
    }

    public ConfiguredValueTaskAwaitable<bool> HealthCheckAsync(CancellationToken ct)
    {
        return HealthCheckCore(ct).ConfigureAwait(false);
    }

    public bool HealthCheck()
    {
        try
        {
            GetRequest(new());

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask<bool> HealthCheckCore(CancellationToken ct)
    {
        try
        {
            await GetRequestAsync(new(), ct);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private TGetResponse GetRequest(TGetRequest request)
    {
        var headers = _headersFactory.Create();

        using var httpResponse = _httpClient
            .SetHeaders(headers.Span)
            .PostAsJson(RouteHelper.Get, request, _options);

        var response = httpResponse.Content.ReadFromJson<TGetResponse>(_options);

        return response.ThrowIfNull();
    }

    private async ValueTask<TGetResponse> GetRequestAsync(TGetRequest request, CancellationToken ct)
    {
        var headers = _headersFactory.Create();

        using var httpResponse = await _httpClient
            .SetHeaders(headers.Span)
            .PostAsJsonAsync(RouteHelper.Get, request, _options, ct);

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

        var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(_options, ct);

        return response.ThrowIfNull();
    }
}

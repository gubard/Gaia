using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : notnull, new()
    where TPostResponse : notnull, new()
{
    ConfiguredValueTaskAwaitable<Result<TGetResponse>> GetAsync(
        TGetRequest request,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<TPostResponse>> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    );
}

public interface IHttpService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>,
        IHealthCheck
    where TGetResponse : notnull, new()
    where TPostResponse : notnull, new();

public abstract class HttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    : IHttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : notnull, new()
    where TPostResponse : notnull, new()
    where TGetRequest : notnull, new()
{
    public ConfiguredValueTaskAwaitable<Result<TGetResponse>> GetAsync(
        TGetRequest request,
        CancellationToken ct
    )
    {
        return _tryPolicyService.TryAsync(() => GetRequestAsync(request, ct).ConfigureAwait(false));
    }

    public ConfiguredValueTaskAwaitable<Result<TPostResponse>> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        return _tryPolicyService.TryAsync(() =>
            PostRequestAsync(idempotentId, request, ct).ConfigureAwait(false)
        );
    }

    public ConfiguredValueTaskAwaitable<Result> HealthCheckAsync(CancellationToken ct)
    {
        return HealthCheckCore(ct).ConfigureAwait(false);
    }

    protected HttpService(
        IFactory<HttpClient> httpClientFactory,
        JsonSerializerOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory
    )
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _tryPolicyService = tryPolicyService;
        _headersFactory = headersFactory;
    }

    protected abstract TGetRequest CreateHealthCheckGetRequest();

    private readonly IFactory<HttpClient> _httpClientFactory;
    private readonly JsonSerializerOptions _options;
    private readonly ITryPolicyService _tryPolicyService;
    private readonly IFactory<Memory<HttpHeader>> _headersFactory;

    private async ValueTask<Result> HealthCheckCore(CancellationToken ct)
    {
        var response = await GetAsync(CreateHealthCheckGetRequest(), ct);

        return response switch
        {
            TGetResponse => Success.Default,
            ValidationError error => error,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private async ValueTask<Result<TGetResponse>> GetRequestAsync(
        TGetRequest request,
        CancellationToken ct
    )
    {
        using var client = _httpClientFactory.Create();
        var headers = _headersFactory.Create();

        var massage = await client
            .SetHeaders(headers.Span)
            .TryPostAsJsonAsync(RouteHelper.Get, request, _options, ct);

        if (massage is ValidationError error)
        {
            return error;
        }

        using var httpResponse = massage switch
        {
            HttpResponseMessage hrm => hrm,
            _ => throw new ArgumentOutOfRangeException(),
        };

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new UnauthorizedValidationError();
        }

        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadFromJsonAsync<TGetResponse>(_options, ct);

        return response.ThrowIfNull();
    }

    private async ValueTask<Result<TPostResponse>> PostRequestAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        using var client = _httpClientFactory.Create();
        var headers = _headersFactory.Create();

        var massage = await client
            .SetHeaders(headers.Span)
            .AddHeader(new(HttpHeader.IdempotentId, idempotentId.ToString()))
            .TryPostAsJsonAsync(RouteHelper.Post, request, _options, ct);

        if (massage is ValidationError error)
        {
            return error;
        }

        using var httpResponse = massage switch
        {
            HttpResponseMessage hrm => hrm,
            _ => throw new ArgumentOutOfRangeException(),
        };

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new UnauthorizedValidationError();
        }

        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(_options, ct);

        return response.ThrowIfNull();
    }
}

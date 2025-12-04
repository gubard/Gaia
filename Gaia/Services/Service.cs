using System.Net.Http.Json;
using System.Text.Json;
using Gaia.Helpers;

namespace Gaia.Services;

public interface IService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse> where TGetResponse : IValidationErrors, new() where TPostResponse : IValidationErrors, new()
{
    ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct);
    ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct);
}

public abstract class Service<TGetRequest, TPostRequest, TGetResponse, TPostResponse> : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse> where TGetResponse : IValidationErrors, new() where TPostResponse : IValidationErrors, new()
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ITryPolicyService _tryPolicyService;

    protected Service(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions, ITryPolicyService tryPolicyService)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _tryPolicyService = tryPolicyService;
    }

    public ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct)
    {
        return _tryPolicyService.TryAsync(async () =>
        {
            using var httpResponse = await _httpClient.PostAsJsonAsync(RouteHelper.Get, request, _jsonSerializerOptions, ct);
            var response = await httpResponse.Content.ReadFromJsonAsync<TGetResponse>(_jsonSerializerOptions, ct);

            return response.ThrowIfNull();
        });
    }

    public ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct)
    {
        return _tryPolicyService.TryAsync(async () =>
        {
            using var httpResponse = await _httpClient.PostAsJsonAsync(RouteHelper.Post, request, _jsonSerializerOptions, ct);
            var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(_jsonSerializerOptions, ct);

            return response.ThrowIfNull();
        });
    }
}
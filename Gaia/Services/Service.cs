using System.Net.Http.Json;
using System.Text.Json;
using Gaia.Helpers;

namespace Gaia.Services;

public interface IService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
{
    ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct);
    ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct);
}

public abstract class Service<TGetRequest, TPostRequest, TGetResponse, TPostResponse> : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    protected Service(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct)
    {
        using var httpResponse = await _httpClient.PostAsJsonAsync(RouteHelper.Get, request, _jsonSerializerOptions, ct);
        var response = await httpResponse.Content.ReadFromJsonAsync<TGetResponse>(_jsonSerializerOptions, ct);

        return response.ThrowIfNull();
    }

    public async ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct)
    {
        using var httpResponse = await _httpClient.PostAsJsonAsync(RouteHelper.Post, request, _jsonSerializerOptions, ct);
        var response = await httpResponse.Content.ReadFromJsonAsync<TPostResponse>(_jsonSerializerOptions, ct);

        return response.ThrowIfNull();
    }
}
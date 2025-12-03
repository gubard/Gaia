namespace Gaia.Services;

public interface IService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
{
    public ValueTask<TGetResponse> GetAsync(TGetRequest request, CancellationToken ct);
    public ValueTask<TPostResponse> PostAsync(TPostRequest request, CancellationToken ct);
}
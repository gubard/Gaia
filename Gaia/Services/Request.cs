namespace Gaia.Services;

public interface IPostRequest
{
    long LastLocalId { get; set; }
}

public interface IGetRequest
{
    long LastId { get; set; }
}
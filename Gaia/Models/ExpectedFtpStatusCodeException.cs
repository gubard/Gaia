using Gaia.Services;

namespace Gaia.Models;

public sealed class ExpectedFtpStatusCodeException : Exception
{
    public ExpectedFtpStatusCodeException(FtpStatusCode statusCode)
        : base($"Unexpected status code: {statusCode}")
    {
        StatusCode = statusCode;
    }

    public FtpStatusCode StatusCode { get; }
}

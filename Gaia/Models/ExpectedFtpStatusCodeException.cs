using Gaia.Services;

namespace Gaia.Models;

public sealed class ExpectedFtpStatusCodeException : Exception
{
    public ExpectedFtpStatusCodeException(
        FtpStatusCode actualStatusCode,
        FtpStatusCode expectedStatusCode
    )
        : base($"Unexpected status code: {actualStatusCode}. Expected: {expectedStatusCode}.")
    {
        ActualStatusCode = actualStatusCode;
        ExpectedStatusCode = expectedStatusCode;
    }

    public FtpStatusCode ActualStatusCode { get; }
    public FtpStatusCode ExpectedStatusCode { get; }
}

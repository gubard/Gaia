namespace Gaia.Models;

public sealed class ExpectedFtpStatusCodeException : Exception
{
    public ExpectedFtpStatusCodeException(string actualStatusCode, int expectedStatusCode)
        : base($"Unexpected status code: \"{actualStatusCode}\". Expected: {expectedStatusCode}.")
    {
        ActualStatusCode = actualStatusCode;
        ExpectedStatusCode = expectedStatusCode;
    }

    public string ActualStatusCode { get; }
    public int ExpectedStatusCode { get; }
}

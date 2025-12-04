namespace Gaia.Models;

public abstract class ValidationError;

public sealed class ExceptionsValidationError : ValidationError
{
    public ExceptionsValidationError(Exception[] exceptions)
    {
        Exceptions = exceptions;
    }

    public IReadOnlyList<Exception> Exceptions { get; }
}
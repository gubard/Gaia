namespace Gaia.Models;

public abstract class ValidationError;

public sealed class FixedSizeValidationError : ValidationError
{
    public FixedSizeValidationError(uint fixedSize, uint actualSize)
    {
        FixedSize = fixedSize;
        ActualSize = actualSize;
    }

    public uint ActualSize { get; }
    public uint FixedSize { get; }
}

public sealed class ContainsInvalidValueValidationError<T> : ValidationError
{
    public ContainsInvalidValueValidationError(T invalidValue, T[] validValues)
    {
        InvalidValue = invalidValue;
        ValidValues = validValues;
    }

    public T InvalidValue { get; }
    public T[] ValidValues { get; }
}

public sealed class ExceptionsValidationError : ValidationError
{
    public ExceptionsValidationError(Exception[] exceptions)
    {
        Exceptions = exceptions;
    }

    public IReadOnlyList<Exception> Exceptions { get; }
}

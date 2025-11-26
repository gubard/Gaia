namespace Gaia.Errors;

public abstract class PropertyValidationError : ValidationError
{
    protected PropertyValidationError(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}

public sealed class PropertyContainsInvalidValueValidationError<T> : PropertyValidationError
{
    public PropertyContainsInvalidValueValidationError(string propertyName, T invalidValue, T[] validValues) : base(propertyName)
    {
        InvalidValue = invalidValue;
        ValidValues = validValues;
    }

    public T InvalidValue { get; }
    public T[] ValidValues { get; }
}

public sealed class PropertyMaxSizeValidationError : PropertyValidationError
{
    public PropertyMaxSizeValidationError(string propertyName, ulong actualSize, ulong maxSize) : base(propertyName)
    {
        ActualSize = actualSize;
        MaxSize = maxSize;
    }

    public ulong MaxSize { get; }
    public ulong ActualSize { get; }
}

public sealed class PropertyMinSizeValidationError : PropertyValidationError
{
    public PropertyMinSizeValidationError(string propertyName, ulong actualSize, ulong minSize) : base(propertyName)
    {
        ActualSize = actualSize;
        MinSize = minSize;
    }

    public ulong MinSize { get; }
    public ulong ActualSize { get; }
}

public sealed class PropertyNotEqualValidationError : PropertyValidationError
{
    public PropertyNotEqualValidationError(string propertyName, string secondPropertyName) : base(propertyName)
    {
        SecondPropertyName = secondPropertyName;
    }

    public string SecondPropertyName { get; }
}

public sealed class PropertyZeroValidationError(string propertyName) : PropertyValidationError(propertyName);
public sealed class PropertyEmptyValidationError(string propertyName) : PropertyValidationError(propertyName);
public sealed class PropertyInvalidValidationError(string propertyName) : PropertyValidationError(propertyName);
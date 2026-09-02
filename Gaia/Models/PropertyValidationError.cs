using Gaia.Services;

namespace Gaia.Models;

public abstract class PropertyValidationError : ValidationError
{
    protected PropertyValidationError(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}

public sealed class PropertyZeroValidationError(string propertyName)
    : PropertyValidationError(propertyName);

public sealed class PropertyEmptyValidationError(string propertyName)
    : PropertyValidationError(propertyName);

public sealed class PropertyInvalidValidationError(string propertyName)
    : PropertyValidationError(propertyName);

public sealed class PropertyValueValidationError : PropertyValidationError
{
    public PropertyValueValidationError(string propertyName, ValidationError validationError)
        : base(propertyName)
    {
        ValidationError = validationError;
    }

    public ValidationError ValidationError { get; }
}

public sealed class PropertyTheDateHasExpiredValidationError : PropertyValidationError
{
    public PropertyTheDateHasExpiredValidationError(
        string propertyName,
        DateOnly actualDate,
        DateOnly expireDate
    )
        : base(propertyName)
    {
        ActualDate = actualDate;
        ExpireDate = expireDate;
    }

    public DateOnly ActualDate { get; }
    public DateOnly ExpireDate { get; }
}

public sealed class PropertyMaxSizeValidationError
    : PropertyValidationError,
        IObjectPropertyStringValueGetter
{
    public PropertyMaxSizeValidationError(string propertyName, ulong actualSize, ulong maxSize)
        : base(propertyName)
    {
        ActualSize = actualSize;
        MaxSize = maxSize;
    }

    public ulong MaxSize { get; }
    public ulong ActualSize { get; }

    public string? FindStringValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(MaxSize) => MaxSize.ToString(),
            nameof(ActualSize) => ActualSize.ToString(),
            nameof(PropertyName) => PropertyName,
            _ => null,
        };
    }
}

public sealed class PropertyStartWithValidationError
    : PropertyValidationError,
        IObjectPropertyStringValueGetter
{
    public PropertyStartWithValidationError(string propertyName, string startWith)
        : base(propertyName)
    {
        StartWith = startWith;
    }

    public string StartWith { get; }

    public string? FindStringValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(StartWith) => StartWith,
            nameof(PropertyName) => PropertyName,
            _ => null,
        };
    }
}

public sealed class PropertyMinSizeValidationError
    : PropertyValidationError,
        IObjectPropertyStringValueGetter
{
    public PropertyMinSizeValidationError(string propertyName, ulong actualSize, ulong minSize)
        : base(propertyName)
    {
        ActualSize = actualSize;
        MinSize = minSize;
    }

    public ulong MinSize { get; }
    public ulong ActualSize { get; }

    public string? FindStringValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(MinSize) => MinSize.ToString(),
            nameof(ActualSize) => ActualSize.ToString(),
            nameof(PropertyName) => PropertyName,
            _ => null,
        };
    }
}

public sealed class PropertyNotEqualPropertyValidationError
    : PropertyValidationError,
        IObjectPropertyStringValueGetter
{
    public PropertyNotEqualPropertyValidationError(string propertyName, string secondPropertyName)
        : base(propertyName)
    {
        SecondPropertyName = secondPropertyName;
    }

    public string SecondPropertyName { get; }

    public string? FindStringValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(PropertyName) => PropertyName,
            nameof(SecondPropertyName) => SecondPropertyName,
            _ => null,
        };
    }
}

public sealed class PropertyEqualValueValidationError
    : PropertyValidationError,
        IObjectPropertyStringValueGetter
{
    public PropertyEqualValueValidationError(string propertyName, object value)
        : base(propertyName)
    {
        Value = value;
    }

    public object Value { get; }

    public string? FindStringValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(PropertyName) => PropertyName,
            nameof(Value) => Value.ToString(),
            _ => null,
        };
    }
}

public sealed class PropertyEqualValidationError
    : PropertyValidationError,
        IObjectPropertyStringValueGetter
{
    public PropertyEqualValidationError(string propertyName, string propertyName2)
        : base(propertyName)
    {
        PropertyName2 = propertyName2;
    }

    public string PropertyName2 { get; }

    public string? FindStringValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(PropertyName) => PropertyName,
            nameof(PropertyName2) => PropertyName2,
            _ => null,
        };
    }
}

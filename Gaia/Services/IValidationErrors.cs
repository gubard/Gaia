using Gaia.Models;

namespace Gaia.Services;

public interface IValidationErrors
{
    List<ValidationError> ValidationErrors { get; }
}

public sealed class EmptyValidationErrors : IValidationErrors
{
    public static IValidationErrors Instance = new EmptyValidationErrors();

    public List<ValidationError> ValidationErrors => new();
}

public sealed class DefaultValidationErrors : IValidationErrors
{
    public List<ValidationError> ValidationErrors { get; } = new();
}

using Gaia.Models;

namespace Gaia.Services;

public interface IValidationErrors
{
    List<ValidationError> ValidationErrors { get; }
}

public class DefaultValidationErrors : IValidationErrors
{
    public List<ValidationError> ValidationErrors => new();
}

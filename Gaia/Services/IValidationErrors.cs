using Gaia.Models;

namespace Gaia.Services;

public interface IValidationErrors
{
    List<ValidationError> ValidationErrors { get; }
}

public class EmptyValidationErrors : IValidationErrors
{
    public List<ValidationError> ValidationErrors => new();
}

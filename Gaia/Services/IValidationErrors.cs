using Gaia.Errors;

namespace Gaia.Services;

public interface IValidationErrors
{
    ValidationError[] ValidationErrors { get; }
}
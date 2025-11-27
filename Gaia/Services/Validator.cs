using Gaia.Errors;

namespace Gaia.Services;

public interface IValidator<in TValue>
{
    ValidationError[] Validate(TValue value, string identity);
}
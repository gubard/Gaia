using System.Runtime.CompilerServices;
using Gaia.Services;

namespace Gaia.Helpers;

public static class ValidationErrorsExtension
{
    public static DefaultValidationErrors Combine(this IValidationErrors[] validationErrors)
    {
        var result = new DefaultValidationErrors();

        foreach (var validationError in validationErrors)
        {
            result.ValidationErrors.AddRange(validationError.ValidationErrors);
        }

        return result;
    }

    public static async ValueTask<IValidationErrors> ToValidationErrors<T>(
        this ConfiguredValueTaskAwaitable<T> task
    )
        where T : IValidationErrors
    {
        var result = await task;

        return result;
    }
}

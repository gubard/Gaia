using System.Diagnostics.CodeAnalysis;

namespace Gaia.Models;

public readonly struct Success
{
    public static Success Default = new();
}

public union Result<T>(T, ValidationError)
    where T : notnull
{
    public Result ToResult()
    {
        return this switch
        {
            T => Result.Ok,
            ValidationError error => error,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public bool TryGet(
        [NotNullWhen(true)] out T? value,
        [NotNullWhen(false)] out ValidationError? error
    )
    {
        if (this is ValidationError e)
        {
            error = e;
            value = default;

            return false;
        }

        error = null;
        value = (T)Value;

        return true;
    }
}

public union Result(Success, ValidationError)
{
    public static Result Ok = new(Success.Default);
}

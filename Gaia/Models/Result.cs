namespace Gaia.Models;

public readonly struct Success
{
    public static Success Default = new ();
}

public union Result<T>(T, ValidationError) where T : notnull
{
    public Result ToResult()
    {
          return this switch {
            T  => Result.Ok,
            ValidationError error =>error,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public T GetValue()
    {
        return this switch {
            T value => value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public union Result(Success, ValidationError)
{
    public static Result Ok = new (Success.Default);
}

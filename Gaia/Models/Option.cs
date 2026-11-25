namespace Gaia.Models;

public readonly struct None
{
    public static None Default = new();
}

public union Option<T>(T, None)
    where T : class
{
    public static Option<T> None = new(Gaia.Models.None.Default);

    public T? GetValue()
    {
        return this switch
        {
            T value => value,
            Gaia.Models.None => null,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

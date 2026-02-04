namespace Gaia.Helpers;

public static class FuncHelper
{
    public static readonly Action Empty = () => { };
}

public static class FuncHelper<T>
{
    public static readonly Action<T> Empty = _ => { };
}

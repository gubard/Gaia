namespace Gaia.Models;

public sealed class Named<TValue>
{
    public Named(string name, TValue value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public TValue Value { get; }
}

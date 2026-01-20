namespace Gaia.Models;

public interface IId<out T>
{
    public T Id { get; }
}

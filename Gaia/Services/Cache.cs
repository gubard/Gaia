namespace Gaia.Services;

public interface ICache<in TSource>
{
    void Update(TSource source);
}

public class EmptyCache<TSource> : ICache<TSource>
{
    public static readonly EmptyCache<TSource> Instance = new();

    public void Update(TSource source) { }
}

public abstract class Cache<TSource, TItem> : ICache<TSource>
    where TItem : IStaticFactory<Guid, TItem>
{
    protected readonly Dictionary<Guid, TItem> Items = new();

    public abstract void Update(TSource source);

    protected TItem GetItem(Guid id)
    {
        if (Items.TryGetValue(id, out var value))
        {
            return value;
        }

        var result = TItem.Create(id);

        if (Items.TryAdd(id, result))
        {
            return result;
        }

        return Items[id];
    }
}

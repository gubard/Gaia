namespace Gaia.Services;

public interface IMemoryCache<in TSource>
{
    void Update(TSource source);
}

public class EmptyMemoryCache<TSource> : IMemoryCache<TSource>
{
    public static readonly EmptyMemoryCache<TSource> Instance = new();

    public void Update(TSource source) { }
}

public abstract class MemoryCache<TSource, TItem> : IMemoryCache<TSource>
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

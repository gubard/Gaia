using System.Runtime.CompilerServices;

namespace Gaia.Services;

public interface IMemoryCache<in TPostRequest, in TGetResponse>
    : ICache<TPostRequest, TGetResponse>;

public abstract class MemoryCache<TItem, TPostRequest, TGetResponse>
    : IMemoryCache<TPostRequest, TGetResponse>
    where TItem : IStaticFactory<Guid, TItem>
{
    public abstract void Update(TPostRequest source);
    public abstract void Update(TGetResponse source);

    public abstract ConfiguredValueTaskAwaitable UpdateAsync(
        TPostRequest source,
        CancellationToken ct
    );

    public abstract ConfiguredValueTaskAwaitable UpdateAsync(
        TGetResponse source,
        CancellationToken ct
    );

    protected readonly Dictionary<Guid, TItem> Items = new();

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

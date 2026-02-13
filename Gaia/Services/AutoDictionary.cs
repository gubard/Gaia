using Gaia.Models;

namespace Gaia.Services;

public sealed class AutoDictionary<TKey, TItem>
    where TItem : IStaticFactory<TKey, TItem>, IId<TKey>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TItem> _items = new();

    public TItem GetItem(TKey id)
    {
        if (_items.TryGetValue(id, out var value))
        {
            return value;
        }

        var result = TItem.Create(id);

        if (_items.TryAdd(id, result))
        {
            return result;
        }

        return _items[id];
    }

    public void AddRange(IEnumerable<TItem> items)
    {
        foreach (var item in items)
        {
            _items.Add(item.Id, item);
        }
    }

    public TItem[] ToItemsArray()
    {
        return _items.Values.ToArray();
    }
}

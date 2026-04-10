using System.Runtime.CompilerServices;
using Gaia.Helpers;

namespace Gaia.Services;

public interface IObjectStorage
{
    ConfiguredValueTaskAwaitable<T> LoadAsync<T>(string key, CancellationToken ct)
        where T : new();

    ConfiguredValueTaskAwaitable SaveAsync(string key, object obj, CancellationToken ct);
}

using System.Collections.Frozen;

namespace Gaia.Services;

public interface IFactory<in TInput, out TOutput>
{
    TOutput Create(TInput input);
}

public interface IFactory<out TOutput>
{
    TOutput Create();
}

public class HashServiceFactory : IFactory<string, IHashService<string, string>>
{
    private readonly FrozenDictionary<string, IHashService<string, string>> _hashServices;

    public HashServiceFactory(FrozenDictionary<string, IHashService<string, string>> hashServices)
    {
        _hashServices = hashServices;
    }

    public IHashService<string, string> Create(string input)
    {
        return _hashServices[input];
    }
}
using System.Collections.Frozen;
using Gaia.Models;

namespace Gaia.Services;

public interface IStaticFactory<in TInput, out TOutput>
{
    static abstract TOutput Create(TInput input);
}

public interface IFactory<in TInput, out TOutput>
{
    TOutput Create(TInput input);
}

public interface IFactory<out TOutput>
{
    TOutput Create();
}

public sealed class EmptyHeadersFactory : IFactory<Memory<HttpHeader>>
{
    public static readonly EmptyHeadersFactory Instance = new();

    public Memory<HttpHeader> Create()
    {
        return Memory<HttpHeader>.Empty;
    }
}

public sealed class HashServiceFactory : IFactory<string, IHashService<string, string>>
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

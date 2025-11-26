using System.Security.Cryptography;

namespace Gaia.Services;

public interface IHashService<in TInput, out TOutput>
{
    TOutput ComputeHash(TInput input);
}

public class Sha512HashService : IHashService<byte[], byte[]>, IDisposable
{
    private readonly SHA512 _sha512;

    public Sha512HashService(SHA512 sha512)
    {
        _sha512 = sha512;
    }

    public byte[] ComputeHash(byte[] input)
    {
        return _sha512.ComputeHash(input);
    }

    public void Dispose()
    {
        _sha512.Dispose();
    }
}

public class StringHashService : IHashService<string, string>
{
    private readonly IHashService<byte[], byte[]> _hashService;
    private readonly ITransformer<string, byte[]> _stingToBytesTransformer;
    private readonly ITransformer<byte[], string> _bytesToStringTransformer;

    public StringHashService(IHashService<byte[], byte[]> hashService,
        ITransformer<string, byte[]> stingToBytesTransformer, ITransformer<byte[], string> bytesToStringTransformer)
    {
        _hashService = hashService;
        _stingToBytesTransformer = stingToBytesTransformer;
        _bytesToStringTransformer = bytesToStringTransformer;
    }

    public string ComputeHash(string input)
    {
        var bytes = _stingToBytesTransformer.Transform(input);
        var hash = _hashService.ComputeHash(bytes);

        return _bytesToStringTransformer.Transform(hash);
    }
}
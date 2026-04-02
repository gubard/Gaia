using System.Security.Cryptography;

namespace Gaia.Services;

public interface IHashService<in TInput, out TOutput>
{
    TOutput ComputeHash(TInput input);
}

public sealed class Sha512HashService : IHashService<byte[], byte[]>, IDisposable
{
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

    private readonly SHA512 _sha512;
}

public sealed class StringHashService : IHashService<string, string>
{
    private readonly IHashService<byte[], byte[]> _hashService;
    private readonly ITransformer<string, byte[]> _stingToBytesTransformer;
    private readonly ITransformer<byte[], string> _bytesToStringTransformer;

    public StringHashService(
        IHashService<byte[], byte[]> hashService,
        ITransformer<string, byte[]> stingToBytesTransformer,
        ITransformer<byte[], string> bytesToStringTransformer
    )
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

public sealed class BytesToStringHashService : IHashService<byte[], string>
{
    public BytesToStringHashService(
        ITransformer<byte[], string> bytesToStringTransformer,
        IHashService<byte[], byte[]> bytesHashService
    )
    {
        _bytesToStringTransformer = bytesToStringTransformer;
        _bytesHashService = bytesHashService;
    }

    public string ComputeHash(byte[] input)
    {
        var bytes = _bytesHashService.ComputeHash(input);
        return _bytesToStringTransformer.Transform(bytes);
    }

    private readonly IHashService<byte[], byte[]> _bytesHashService;
    private readonly ITransformer<byte[], string> _bytesToStringTransformer;
}

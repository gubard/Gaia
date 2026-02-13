using System.Text;

namespace Gaia.Services;

public interface ITransformer<in TInput, out TOutput>
{
    TOutput Transform(TInput input);
}

public abstract class StringToEncoding : ITransformer<string, byte[]>
{
    public byte[] Transform(string input)
    {
        return _encoding.GetBytes(input);
    }

    protected StringToEncoding(Encoding encoding)
    {
        _encoding = encoding;
    }

    private readonly Encoding _encoding;
}

public sealed class StringToUtf8 : StringToEncoding
{
    public StringToUtf8()
        : base(Encoding.UTF8) { }
}

public sealed class BytesToHex : ITransformer<byte[], string>
{
    public string Transform(byte[] input)
    {
        return Convert.ToHexString(input);
    }
}

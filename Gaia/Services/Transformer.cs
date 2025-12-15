using System.Text;

namespace Gaia.Services;

public interface ITransformer<in TInput, out TOutput>
{
    TOutput Transform(TInput input);
}

public abstract class StringToEncoding : ITransformer<string, byte[]>
{
    private readonly Encoding _encoding;

    public StringToEncoding(Encoding encoding)
    {
        _encoding = encoding;
    }

    public byte[] Transform(string input)
    {
        return _encoding.GetBytes(input);
    }
}

public sealed class StringToUtf8 : StringToEncoding
{
    public StringToUtf8()
        : base(Encoding.UTF8) { }
}

public class BytesToHex : ITransformer<byte[], string>
{
    public string Transform(byte[] input)
    {
        return Convert.ToHexString(input);
    }
}

using System.Text;

namespace Gaia.Services;

public interface ITransformer<in TInput, out TOutput>
{
    TOutput Transform(TInput input);
}

public class StringToBytes : ITransformer<string, byte[]>
{
    private readonly Encoding _encoding;

    public StringToBytes(Encoding encoding)
    {
        _encoding = encoding;
    }

    public byte[] Transform(string input)
    {
        return _encoding.GetBytes(input);
    }
}

public class StringToUtf8 : StringToBytes
{
    public StringToUtf8() : base(Encoding.UTF8)
    {
    }
}
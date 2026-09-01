namespace Gaia.Helpers;

public static class ByteExtension
{
    public static Stream ToStream(this IReadOnlyList<byte> bytes)
    {
        var stream = new MemoryStream();
        stream.Write(bytes.ToArray());
        stream.Position = 0;

        return stream;
    }
}

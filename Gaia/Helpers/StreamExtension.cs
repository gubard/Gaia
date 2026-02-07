namespace Gaia.Helpers;

public static class StreamExtension
{
    public static Span<byte> ToByteSpan(this Stream stream)
    {
        Span<byte> buffer = new byte[stream.Length];
        stream.ReadExactly(buffer);

        return buffer;
    }

    public static async Task<Memory<byte>> ToByteMemoryAsync(this Stream stream)
    {
        var buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer);

        return buffer;
    }
}

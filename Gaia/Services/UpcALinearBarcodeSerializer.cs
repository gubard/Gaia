using System.Collections.Frozen;

namespace Gaia.Services;

public interface ILinearBarcodeSerializer
{
    string Name { get; }
    Span<bool> Serialize(ReadOnlySpan<char> barcode);
}

public sealed class UpcALinearBarcodeSerializer : ILinearBarcodeSerializer
{
    public string Name => "UPC-A";

    public Span<bool> Serialize(ReadOnlySpan<char> barcode)
    {
        Span<bool> result = new bool[BarcodeSize];
        QuietZone.Span.CopyTo(result.Slice(0, 10));
        GuardPattern.Span.CopyTo(result.Slice(10, 3));
        LeftSidePattern[barcode[0]].Span.CopyTo(result.Slice(13, 7));
        LeftSidePattern[barcode[1]].Span.CopyTo(result.Slice(20, 7));
        LeftSidePattern[barcode[2]].Span.CopyTo(result.Slice(27, 7));
        LeftSidePattern[barcode[3]].Span.CopyTo(result.Slice(34, 7));
        LeftSidePattern[barcode[4]].Span.CopyTo(result.Slice(41, 7));
        LeftSidePattern[barcode[5]].Span.CopyTo(result.Slice(48, 7));
        CenterGuardPattern.Span.CopyTo(result.Slice(53, 5));
        RightSidePattern[barcode[6]].Span.CopyTo(result.Slice(58, 7));
        RightSidePattern[barcode[7]].Span.CopyTo(result.Slice(65, 7));
        RightSidePattern[barcode[8]].Span.CopyTo(result.Slice(72, 7));
        RightSidePattern[barcode[9]].Span.CopyTo(result.Slice(79, 7));
        RightSidePattern[barcode[10]].Span.CopyTo(result.Slice(86, 7));
        RightSidePattern[barcode[11]].Span.CopyTo(result.Slice(94, 7));
        GuardPattern.Span.CopyTo(result.Slice(101, 3));
        QuietZone.Span.CopyTo(result.Slice(104, 10));

        return result;
    }

    private static readonly ReadOnlyMemory<bool> GuardPattern = new[] { true, false, true };
    private static readonly FrozenDictionary<char, ReadOnlyMemory<bool>> LeftSidePattern;
    private static readonly FrozenDictionary<char, ReadOnlyMemory<bool>> RightSidePattern;
    private const int BarcodeSize = 10 * 2 + 3 * 2 + 5 + 6 * 2 * 7;

    private static readonly ReadOnlyMemory<bool> CenterGuardPattern = new[]
    {
        false,
        true,
        false,
        true,
        false,
    };

    private static readonly ReadOnlyMemory<bool> QuietZone = new[]
    {
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
    };

    static UpcALinearBarcodeSerializer()
    {
        LeftSidePattern = new Dictionary<char, ReadOnlyMemory<bool>>
        {
            { '0', new[] { false, false, false, true, true, false, true } },
            { '1', new[] { false, false, true, true, false, false, true } },
            { '2', new[] { false, false, true, false, false, true, true } },
            { '3', new[] { false, true, true, true, true, false, true } },
            { '4', new[] { false, true, false, false, false, true, true } },
            { '5', new[] { false, true, true, false, false, false, true } },
            { '6', new[] { false, true, false, true, true, true, true } },
            { '7', new[] { false, true, true, true, false, true, true } },
            { '8', new[] { false, true, true, false, true, true, true } },
            { '9', new[] { false, false, false, true, false, true, true } },
        }.ToFrozenDictionary();

        var rightSidePattern = new Dictionary<char, ReadOnlyMemory<bool>>();

        foreach (var value in LeftSidePattern)
        {
            rightSidePattern.Add(value.Key, ReversePattern(value.Value));
        }

        RightSidePattern = rightSidePattern.ToFrozenDictionary();
    }

    private static ReadOnlyMemory<bool> ReversePattern(ReadOnlyMemory<bool> memory)
    {
        Memory<bool> result = memory.ToArray();

        for (int i = 0; i < result.Length; i++)
        {
            result.Span[i] = !result.Span[i];
        }

        return result;
    }
}

using System.Buffers;
using System.Collections.Frozen;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface ILinearBarcodeSerializer
{
    string Name { get; }
    byte BarWidth { get; }

    Span<bool> Serialize(ReadOnlySpan<char> barcode);
    IEnumerable<ValidationError> Validate(ReadOnlySpan<char> barcode);
}

public sealed class CodabarLinearBarcodeSerializer : ILinearBarcodeSerializer
{
    public string Name => "Codabar";
    public byte BarWidth => 2;

    public Span<bool> Serialize(ReadOnlySpan<char> barcode)
    {
        Span<bool> result = new bool[
            MinSize + barcode.Sum(x => Pattern[x].Length) + barcode.Length
        ];

        var currentIndex = 0;
        QuietZone.Span.CopyTo(result.Slice(currentIndex, QuietZone.Length));
        currentIndex += QuietZone.Length;
        StartStop.Span.CopyTo(result.Slice(currentIndex, StartStop.Length));
        currentIndex += StartStop.Length;
        Gap.Span.CopyTo(result.Slice(currentIndex, Gap.Length));
        currentIndex += Gap.Length;

        foreach (var value in barcode)
        {
            var pattern = Pattern[value];
            pattern.Span.CopyTo(result.Slice(currentIndex, pattern.Length));
            currentIndex += pattern.Length;
            Gap.Span.CopyTo(result.Slice(currentIndex, Gap.Length));
            currentIndex += Gap.Length;
        }

        StartStop.Span.CopyTo(result.Slice(currentIndex, StartStop.Length));
        currentIndex += StartStop.Length;
        QuietZone.Span.CopyTo(result.Slice(currentIndex, QuietZone.Length));

        return result;
    }

    public IEnumerable<ValidationError> Validate(ReadOnlySpan<char> barcode)
    {
        var index = barcode.IndexOfAnyExcept(ValidValues);

        if (index >= 0)
        {
            return
            [
                new ContainsInvalidValueValidationError<char>(barcode[index], ValidChars.ToArray()),
            ];
        }

        return [];
    }

    private static readonly ReadOnlyMemory<char> ValidChars;
    private static readonly SearchValues<char> ValidValues;
    private static readonly ReadOnlyMemory<bool> Gap = new[] { false };
    private static readonly uint MinSize;

    private static readonly FrozenDictionary<char, ReadOnlyMemory<bool>> Pattern = new Dictionary<
        char,
        ReadOnlyMemory<bool>
    >
    {
        { '0', new[] { false, false, false, false, false, true, true } },
        { '1', new[] { false, false, false, false, true, true, false } },
        { '2', new[] { false, false, false, true, false, false, true } },
        { '3', new[] { true, true, false, false, false, false, false } },
        { '4', new[] { false, false, true, false, false, true, false } },
        { '5', new[] { true, false, false, false, false, true, false } },
        { '6', new[] { false, true, false, false, false, false, true } },
        { '7', new[] { false, true, false, false, true, false, false } },
        { '8', new[] { false, true, true, false, false, false, false } },
        { '9', new[] { true, false, false, true, false, false, false } },
        { '-', new[] { false, false, false, true, true, false, false } },
        { '$', new[] { false, false, true, true, false, false, false } },
        { ':', new[] { true, false, false, false, true, false, true } },
        { '/', new[] { true, false, true, false, false, false, true } },
        { '.', new[] { true, false, true, false, true, false, false } },
        { '+', new[] { false, false, true, false, true, false, true } },
    }.ToFrozenDictionary();

    private static readonly ReadOnlyMemory<bool> StartStop = new[]
    {
        false,
        false,
        true,
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

    static CodabarLinearBarcodeSerializer()
    {
        ValidChars = Pattern.Keys.ToArray();
        ValidValues = SearchValues.Create(ValidChars.ToArray());
        MinSize = (uint)QuietZone.Length * 2 + (uint)StartStop.Length * 2 + (uint)Gap.Length;
    }
}

public sealed class UpcALinearBarcodeSerializer : ILinearBarcodeSerializer
{
    public string Name => "UPC-A";
    public byte BarWidth => 2;

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

    public IEnumerable<ValidationError> Validate(ReadOnlySpan<char> barcode)
    {
        if (barcode.Length != TextSize)
        {
            return [new FixedSizeValidationError(TextSize, (uint)barcode.Length)];
        }

        var index = barcode.IndexOfAnyExcept(ValidValues);

        if (index >= 0)
        {
            return
            [
                new ContainsInvalidValueValidationError<char>(barcode[index], ValidChars.ToArray()),
            ];
        }

        return [];
    }

    private const int BarcodeSize = 10 * 2 + 3 * 2 + 5 + 6 * 2 * 7;
    private const uint TextSize = 12;

    private static readonly ReadOnlyMemory<bool> GuardPattern = new[] { true, false, true };
    private static readonly FrozenDictionary<char, ReadOnlyMemory<bool>> LeftSidePattern;
    private static readonly FrozenDictionary<char, ReadOnlyMemory<bool>> RightSidePattern;
    private static readonly ReadOnlyMemory<char> ValidChars;
    private static readonly SearchValues<char> ValidValues;

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
        ValidChars = LeftSidePattern.Keys.ToArray();
        ValidValues = SearchValues.Create(ValidChars.ToArray());
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

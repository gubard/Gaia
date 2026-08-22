using Gaia.Helpers;

namespace Gaia.Models;

public readonly struct Emu
{
    public Emu(int value)
    {
        _value = value;
    }

    public static implicit operator int(Emu source)
    {
        return source._value;
    }

    public static implicit operator Emu(int source)
    {
        return new Emu(source);
    }

    public static explicit operator Emu(Pixel source)
    {
        return (int)(source * Consts.PixelToEmu);
    }

    public static explicit operator Emu(Centimeter source)
    {
        return (int)(source * Consts.CentimeterToEmu);
    }

    public static explicit operator Emu(Twip source)
    {
        return source * Consts.TwipToEmu;
    }

    private readonly int _value;
}

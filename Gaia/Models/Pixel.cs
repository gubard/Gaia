using Gaia.Helpers;

namespace Gaia.Models;

public readonly struct Pixel
{
    public Pixel(double value)
    {
        _value = value;
    }

    private readonly double _value;

    public static explicit operator Pixel(Twip source)
    {
        return new Pixel(source / Consts.PixelToTwip);
    }

    public static explicit operator Pixel(Centimeter source)
    {
        return source * Consts.CentimeterToPixel;
    }

    public static implicit operator double(Pixel source)
    {
        return source._value;
    }

    public static explicit operator Pixel(Emu source)
    {
        return source / Consts.PixelToEmu;
    }

    public static implicit operator Pixel(double source)
    {
        return new Pixel(source);
    }
}

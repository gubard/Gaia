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
        return (Pixel)(Twip)source;
    }

    public static implicit operator double(Pixel source)
    {
        return source._value;
    }
}
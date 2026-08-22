using Gaia.Helpers;

namespace Gaia.Models;

public readonly struct Centimeter
{
    public Centimeter(double value)
    {
        _value = value;
    }

    public static explicit operator Centimeter(Twip source)
    {
        return new Centimeter((int)source / Consts.CentimeterToTwip);
    }

    public static explicit operator Centimeter(Pixel source)
    {
        return source / Consts.CentimeterToPixel;
    }

    public static implicit operator double(Centimeter source)
    {
        return source._value;
    }

    public static explicit operator Centimeter(int source)
    {
        return new Centimeter(source);
    }

    public static implicit operator Centimeter(double source)
    {
        return new Centimeter(source);
    }

    public static explicit operator Centimeter(Emu source)
    {
        return source / Consts.CentimeterToEmu;
    }

    public static Centimeter operator -(Centimeter x, Centimeter y)
    {
        return new Centimeter(x._value - y._value);
    }

    private readonly double _value;
}

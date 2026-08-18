using Gaia.Helpers;

namespace Gaia.Models;

public readonly struct Twip
{
    public Twip(int value)
    {
        _value = value;
    }

    private readonly int _value;

    public static explicit operator Twip(Centimeter source)
    {
        return new Twip((int)(source * Consts.CentimeterToTwip));
    }

    public static explicit operator Twip(Pixel source)
    {
        return new Twip((int)(source * Consts.PixelToTwip));
    }

    public static implicit operator Twip(int source)
    {
        return new Twip(source);
    }

    public static implicit operator int(Twip source)
    {
        return source._value;
    }

    public static explicit operator uint(Twip source)
    {
        return (uint)source._value;
    }

    public static explicit operator double(Twip source)
    {
        return source._value;
    }

    public static explicit operator Twip(double source)
    {
        return new Twip((int)source);
    }

    public static explicit operator Twip(uint source)
    {
        return new Twip((int)source);
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}

namespace Gaia.Models;

public readonly struct UnitLength
{
    public static readonly UnitLength Zero = new(0, 0, 0, 0);

    public const int TwipToEmu = 635;
    public const double CentimeterToEmu = 360_000;
    public const double PixelToEmu = 9525;
    public const double CentimeterToTwip = 1440.0 / 2.54;
    public const double PixelToTwip = 15;
    public const double CentimeterToPixel = 96.0 / 2.54;

    public readonly double Centimeter;
    public readonly int Emu;
    public readonly int Twip;
    public readonly double Pixel;

    private UnitLength(double centimeter, int emu, int twip, double pixel)
    {
        Centimeter = centimeter;
        Emu = emu;
        Twip = twip;
        Pixel = pixel;
    }

    public static UnitLength SubTwip(UnitLength left, UnitLength right)
    {
        return FromTwip(left.Twip - right.Twip);
    }

    public static UnitLength FromCentimeter(double centimeter)
    {
        return new(
            centimeter,
            (int)(centimeter * CentimeterToEmu),
            (int)(centimeter * CentimeterToTwip),
            centimeter * CentimeterToPixel
        );
    }

    public static UnitLength FromEmu(int emu)
    {
        return new(emu / CentimeterToEmu, emu, emu / TwipToEmu, emu / PixelToEmu);
    }

    public static UnitLength FromTwip(int twip)
    {
        return new(twip / CentimeterToTwip, twip * TwipToEmu, twip, twip / PixelToTwip);
    }

    public static UnitLength FromPixel(double pixel)
    {
        return new(
            pixel / CentimeterToPixel,
            (int)(pixel * PixelToEmu),
            (int)(pixel * PixelToTwip),
            pixel
        );
    }
}

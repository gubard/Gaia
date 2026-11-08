namespace Gaia.Models;

public readonly struct UnitLength
{
    public static readonly UnitLength Zero = new(0, 0, 0, 0, 0);

    public const int TwipToEmu = 635;
    public const double CentimeterToEmu = 360_000;
    public const double PixelToEmu = 9525;
    public const int PointsToEmu = 12700;
    public const double CentimeterToTwip = 1440.0 / 2.54;
    public const double PixelToTwip = 15;
    public const int PointsToTwip = 20;
    public const double CentimeterToPixel = 96.0 / 2.54;
    public const double PointsToPixel = 4.0 / 3.0;
    public const double CentimeterToPoints = 28.3465;

    public readonly double Centimeter;
    public readonly int Emu;
    public readonly int Twip;
    public readonly double Pixel;
    public readonly int Points;

    private UnitLength(double centimeter, int emu, int twip, double pixel, int points)
    {
        Centimeter = centimeter;
        Emu = emu;
        Twip = twip;
        Pixel = pixel;
        Points = points;
    }

    public static UnitLength Min(UnitLength left, UnitLength right)
    {
        return FromPixel(Math.Min(left.Pixel, right.Pixel));
    }

    public static UnitLength SubTwip(UnitLength left, UnitLength right)
    {
        return FromTwip(left.Twip - right.Twip);
    }

    public static UnitLength DivisionPixel(UnitLength left, UnitLength right)
    {
        return FromPixel(left.Pixel / right.Pixel);
    }

    public static UnitLength MultiplicationPixel(UnitLength left, UnitLength right)
    {
        return FromPixel(left.Pixel * right.Pixel);
    }

    public static UnitLength MultiplicationTwip(UnitLength left, int right)
    {
        return FromPixel(left.Twip * right);
    }

    public static UnitLength FromCentimeter(double centimeter)
    {
        return new(
            centimeter,
            (int)(centimeter * CentimeterToEmu),
            (int)(centimeter * CentimeterToTwip),
            centimeter * CentimeterToPixel,
            (int)(centimeter * CentimeterToPoints)
        );
    }

    public static UnitLength FromEmu(int emu)
    {
        return new(
            emu / CentimeterToEmu,
            emu,
            emu / TwipToEmu,
            emu / PixelToEmu,
            emu / PointsToEmu
        );
    }

    public static UnitLength FromTwip(int twip)
    {
        return new(
            twip / CentimeterToTwip,
            twip * TwipToEmu,
            twip,
            twip / PixelToTwip,
            twip / PointsToTwip
        );
    }

    public static UnitLength FromPixel(double pixel)
    {
        return new(
            pixel / CentimeterToPixel,
            (int)(pixel * PixelToEmu),
            (int)(pixel * PixelToTwip),
            pixel,
            (int)(pixel / PointsToPixel)
        );
    }

    public static UnitLength FromPoints(int points)
    {
        return new(
            points / CentimeterToPoints,
            points * PointsToEmu,
            points * PointsToTwip,
            (int)(points * PointsToPixel),
            points
        );
    }
}

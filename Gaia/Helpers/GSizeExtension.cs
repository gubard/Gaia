using Gaia.Models;

namespace Gaia.Helpers;

public static class GSizeExtension
{
    public static GSize FitProportional(this GSize sourceSize, GSize targetSize)
    {
        var scale = UnitLength.Min(
            UnitLength.DivisionPixel(targetSize.Width, sourceSize.Width),
            UnitLength.DivisionPixel(targetSize.Height, sourceSize.Height)
        );

        return new GSize(
            UnitLength.MultiplicationPixel(sourceSize.Width, scale),
            UnitLength.MultiplicationPixel(sourceSize.Height, scale)
        );
    }
}

namespace Gaia.Models;

public readonly struct GSize
{
    public GSize(UnitLength width, UnitLength height)
    {
        Width = width;
        Height = height;
    }

    public readonly UnitLength Width;
    public readonly UnitLength Height;
}

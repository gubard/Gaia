namespace Gaia.Models;

public readonly struct GThickness
{
    public GThickness(UnitLength left, UnitLength top, UnitLength right, UnitLength bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public readonly UnitLength Left;
    public readonly UnitLength Top;
    public readonly UnitLength Right;
    public readonly UnitLength Bottom;
}

namespace Gaia.Models;

public record GaiaValues
{
    public readonly TimeSpan Offset;

    public GaiaValues(TimeSpan offset)
    {
        Offset = offset;
    }
}
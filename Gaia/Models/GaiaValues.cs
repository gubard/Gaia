namespace Gaia.Models;

public record GaiaValues
{
    public readonly TimeSpan Offset;
    public readonly Guid UserId;

    public GaiaValues(TimeSpan offset, Guid userId)
    {
        Offset = offset;
        UserId = userId;
    }
}

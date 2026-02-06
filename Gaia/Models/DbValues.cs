namespace Gaia.Models;

public record DbValues
{
    public readonly TimeSpan Offset;
    public readonly Guid UserId;

    public DbValues(TimeSpan offset, Guid userId)
    {
        Offset = offset;
        UserId = userId;
    }
}

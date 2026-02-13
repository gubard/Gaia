namespace Gaia.Models;

public sealed class ChangeOrder
{
    public Guid StartId { get; set; }
    public Guid[] InsertIds { get; set; } = [];
    public bool IsAfter { get; set; }
}

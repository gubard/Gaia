namespace Gaia.Models;

public interface IDragChangeOrder<TEdit>
    where TEdit : IEdit
{
    ChangeOrder[] ChangeOrders { get; set; }
    TEdit[] Edits { get; set; }
}

public interface IEdit
{
    Guid[] Ids { get; set; }
    bool IsEditParentId { get; set; }
    Guid? ParentId { get; set; }
}

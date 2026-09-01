namespace Gaia.Models;

public sealed class TreeNode<T>
{
    public TreeNode(T node, IEnumerable<TreeNode<T>> children)
    {
        Node = node;
        Children = children;
    }

    public T Node { get; }
    public IEnumerable<TreeNode<T>> Children { get; }
}

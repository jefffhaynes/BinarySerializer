namespace BinarySerialization.Graph;

public abstract class Node<TNode> : IEquatable<Node<TNode>> where TNode : Node<TNode>
{
    private const char PathSeparator = '.';

    protected Node(TNode parent)
    {
        Parent = parent;
    }

    public TNode Parent { get; }

    public string Name { get; protected set; }

    public List<TNode> Children { get; set; }

    public bool Equals(Node<TNode> other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Name, other.Name);
    }

    public TNode GetChild(string path)
    {
        var memberNames = path.Split(PathSeparator);

        if (memberNames.Length == 0) throw new BindingException("Path cannot be empty.");

        var child = (TNode)this;
        foreach (var name in memberNames)
        {
            child = child.Children.SingleOrDefault(c => c.Name == name);

            if (child == null) throw new BindingException($"No field found at '{path}'.");
        }

        return child;
    }

    public override string ToString()
    {
        return Name ?? base.ToString();
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((Node<TNode>)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return Name != null ? Name.GetHashCode() : 0;
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }
}

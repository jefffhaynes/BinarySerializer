namespace BinarySerialization.Graph.TypeGraph;

internal sealed class ArrayTypeNode : CollectionTypeNode
{
    public ArrayTypeNode(TypeNode parent, Type type) : base(parent, type)
    {
    }

    public ArrayTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo)
        : base(parent, parentType, memberInfo)
    {
    }

    internal override ValueNode CreateSerializerOverride(ValueNode parent)
    {
        if (ChildType.GetTypeInfo().IsPrimitive)
        {
            return new PrimitiveArrayValueNode(parent, Name, this);
        }
        return new ArrayValueNode(parent, Name, this);
    }

    protected override Type GetChildType()
    {
        return Type.GetElementType();
    }
}

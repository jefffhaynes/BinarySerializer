using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ArrayTypeNode : CollectionTypeNode
    {
        public ArrayTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            ChildType = Type.GetElementType();
        }

        public ArrayTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo)
            : base(parent, parentType, memberInfo)
        {
            ChildType = Type.GetElementType();
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if (ChildType.IsPrimitive)
                return new PrimitveArrayValueNode(parent, Name, this);
            return new ArrayValueNode(parent, Name, this);
        }
    }
}

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
            Child = GenerateChild(ChildType);
        }

        public ArrayTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            ChildType = Type.GetElementType();
            Child = GenerateChild(ChildType);
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if (ChildType.IsPrimitive)
                return new PrimitveArrayValueNode(parent, Name, this);
            return new ArrayValueNode(parent, Name, this);
        }
    }
}

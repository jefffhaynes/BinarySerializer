using BinarySerialization.Graph.ValueGraph;
using System;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class PackedBooleanArrayTypeNode : TypeNode
    {
        public PackedBooleanArrayTypeNode(TypeNode parent) : base(parent) { }
        public PackedBooleanArrayTypeNode(TypeNode parent, Type type) : base(parent, type) { }
        public PackedBooleanArrayTypeNode(TypeNode parent, Type type, MemberInfo memberInfo) : base(parent, type, memberInfo) { }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
            => new PackedBooleanArrayValueNode(parent, Name, this);

    }
}

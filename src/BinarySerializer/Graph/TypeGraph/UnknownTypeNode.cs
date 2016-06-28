using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class UnknownTypeNode : TypeNode
    {
        public UnknownTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public UnknownTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new UnknownValueNode(parent, Name, this);
        }
    }
}

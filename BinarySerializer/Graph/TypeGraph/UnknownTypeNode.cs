using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    public class UnknownTypeNode : TypeNode
    {
        public UnknownTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public UnknownTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new UnknownValueNode(parent, Name, this);
        }
    }
}

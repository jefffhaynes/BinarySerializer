using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class CustomTypeNode : TypeNode
    {
        public CustomTypeNode(TypeNode parent) : base(parent)
        {
        }

        public CustomTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public CustomTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new CustomValueNode(parent, Name, this);
        }
    }
}

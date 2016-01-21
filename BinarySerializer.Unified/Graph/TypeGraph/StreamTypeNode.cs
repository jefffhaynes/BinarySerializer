using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class StreamTypeNode : TypeNode
    {
        public StreamTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public StreamTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new StreamValueNode(parent, Name, this);
        }
    }
}

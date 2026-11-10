using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class StreamTypeNode : TypeNode
    {
        public StreamTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) : base(parent, type)
        {
        }

        public StreamTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo) : base(parent, parentType,
            memberInfo)
        {
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new StreamValueNode(parent, Name, this);
        }
    }
}

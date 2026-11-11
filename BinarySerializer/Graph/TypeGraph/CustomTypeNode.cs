using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class CustomTypeNode : ObjectTypeNode
    {
        public CustomTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) : base(parent, type)
        {
        }

        public CustomTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo) : base(parent, parentType,
            memberInfo)
        {
        }

        public CustomTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type subType) : base(parent,
            parentType, memberInfo, subType)
        {
        }


        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new CustomValueNode(parent, Name, this);
        }
    }
}

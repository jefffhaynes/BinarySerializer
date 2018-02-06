using System;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class BitFieldTypeNode : ValueTypeNode
    {
        public BitFieldTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public BitFieldTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
        }
    }
}

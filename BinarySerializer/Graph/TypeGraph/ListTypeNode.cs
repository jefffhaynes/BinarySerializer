using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ListTypeNode : CollectionTypeNode
    {
        public ListTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public ListTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType,
            memberInfo)
        {
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if (ChildType.GetTypeInfo().IsPrimitive)
            {
                return new PrimitiveListValueNode(parent, Name, this);
            }
            return new ListValueNode(parent, Name, this);
        }

        protected override Type GetChildType()
        {
            var genericArguments = Type.GetHierarchyGenericArguments().ToList();

            if (genericArguments.Count != 1)
            {
                throw new InvalidOperationException(
                    "Lists must define one and only one generic argument in the list object hierarchy.");
            }

            return genericArguments[0];
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ListTypeNode : CollectionTypeNode
    {
        public ListTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            ChildType = GetChildType(Type);
            Child = GenerateChild(ChildType);
        }

        public ListTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
            ChildType = GetChildType(Type);
            Child = GenerateChild(ChildType);
        }

        private Type GetChildType(Type collectionType)
        {
            if (collectionType.GetGenericArguments().Length > 1)
            {
                throw new NotSupportedException("Multiple generic arguments not supported");
            }

            return collectionType.GetGenericArguments().Single();
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if(ChildType.IsPrimitive)
                return new PrimitiveListValueNode(parent, Name, this);
            return new ListValueNode(parent, Name, this);
        }
    }
}

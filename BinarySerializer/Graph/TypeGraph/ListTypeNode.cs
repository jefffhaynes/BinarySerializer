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
            Construct();
        }

        public ListTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
            Construct();
        }

        private void Construct()
        {
            ChildType = GetChildType(Type);
        }

        private Type GetChildType(Type collectionType)
        {
            return collectionType.GetGenericArguments().Single();
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
#if NET40  //TODO: OR PORTABLE328?
            if(ChildType.IsPrimitive)
                  return new PrimitiveListValueNode(parent, Name, this);
#else
            if (ChildType.GetTypeInfo().IsPrimitive)
                return new PrimitiveListValueNode(parent, Name, this);
#endif

            return new ListValueNode(parent, Name, this);
        }
    }
}

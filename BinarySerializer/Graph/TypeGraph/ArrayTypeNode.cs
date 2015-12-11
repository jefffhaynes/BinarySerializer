using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ArrayTypeNode : CollectionTypeNode
    {
        public ArrayTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            Construct();
        }

        public ArrayTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo)
            : base(parent, parentType, memberInfo)
        {
            Construct();
        }

        private void Construct()
        {
            ChildType = Type.GetElementType();
            CompiledChildConstructor = CreateCompiledConstructor(ChildType);
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if (ChildType.IsPrimitive)
                return new PrimitveArrayValueNode(parent, Name, this);
            return new ArrayValueNode(parent, Name, this);
        }
    }
}

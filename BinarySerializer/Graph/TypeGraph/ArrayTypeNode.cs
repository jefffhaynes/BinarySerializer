using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ArrayTypeNode : CollectionTypeNode
    {
        public ArrayTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) : base(parent, type)
        {
        }

        public ArrayTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo)
            : base(parent, parentType, memberInfo)
        {
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if (ChildType.GetTypeInfo().IsPrimitive)
            {
                return new PrimitiveArrayValueNode(parent, Name, this);
            }
            return new ArrayValueNode(parent, Name, this);
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        [UnconditionalSuppressMessage("Trimming", "IL2072",
            Justification =
                "Array element types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        [UnconditionalSuppressMessage("Trimming", "IL2073",
            Justification =
                "Array element types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        protected override Type GetChildType()
        {
            return Type.GetElementType();
        }
    }
}

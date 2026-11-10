using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ListTypeNode : CollectionTypeNode
    {
        public ListTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) : base(parent, type)
        {
        }

        public ListTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo) : base(parent, parentType,
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

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        [UnconditionalSuppressMessage("Trimming", "IL2072",
            Justification =
                "List element types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        [UnconditionalSuppressMessage("Trimming", "IL2073",
            Justification =
                "List element types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        protected override Type GetChildType()
        {
            var genericArguments = Type.GetHierarchyGenericArguments().Distinct().ToList();

            if (genericArguments.Count != 1)
            {
                throw new InvalidOperationException(
                    "Lists must define one and only one generic argument in the list object hierarchy.");
            }

            return genericArguments[0];
        }
    }
}

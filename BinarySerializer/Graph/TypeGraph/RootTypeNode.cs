using System;
using System.Diagnostics.CodeAnalysis;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class RootTypeNode : ContainerTypeNode
    {
        public RootTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type graphType) : base(parent, graphType)
        {
            Child = GenerateChild(graphType);
        }

        public RootTypeNode([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type graphType)
            : this(null, graphType)
        {
        }

        public TypeNode Child { get; }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new RootValueNode(parent, Name, this);
        }
    }
}

using System;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class RootTypeNode : ContainerTypeNode
    {
        public RootTypeNode(Type graphType) : base(null, graphType)
        {
            Child = GenerateChild(graphType);
        }

        public TypeNode Child { get; private set; }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ContextValueNode(parent, Name, this);
        }

        public override ValueNode CreateSerializer(ValueNode parent)
        {
            return CreateSerializerOverride(parent);
        }
    }
}
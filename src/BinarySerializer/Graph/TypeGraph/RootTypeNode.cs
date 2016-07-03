﻿using System;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class RootTypeNode : ContainerTypeNode
    {
        public RootTypeNode(TypeNode parent, Type graphType) : base(parent, graphType)
        {
            Child = GenerateChild(graphType);
        }

        public RootTypeNode(Type graphType)
            : this(null, graphType)
        {
        }

        public TypeNode Child { get; private set; }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ContextValueNode(parent, Name, this);
        }
    }
}
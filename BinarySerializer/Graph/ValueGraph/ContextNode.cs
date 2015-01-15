using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ContextNode : ValueNode
    {
        public ContextNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public ValueNode Child { get; private set; }

        public override object Value
        {
            get
            {
                return Child.Value;
            }

            set
            {
                Child = ((RootTypeNode)TypeNode).Child.CreateSerializer(this);
                Child.Value = value;
            }
        }

        public object Context
        {
            set
            {
                if (value == null)
                    return;

                var contextGraph = new RootTypeNode(value.GetType());
                var contextSerializer = (ContextNode)contextGraph.CreateSerializer(this);
                contextSerializer.Value = value;

                Children.AddRange(contextSerializer.Child.Children);
            }
        }

        public override void Bind()
        {
            Child.Bind();
        }

        protected override void SerializeOverride(Stream stream)
        {
            Child.Serialize(stream);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            Child = ((RootTypeNode)TypeNode).Child.CreateSerializer(this);
            Child.Deserialize(stream);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ContextValueNode : ValueNode
    {
        private static readonly Dictionary<Type, RootTypeNode> ContextCache = new Dictionary<Type, RootTypeNode>();
        private static readonly object ContextCacheLock = new object();

        public ContextValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public ValueNode Child { get; private set; }

        private RootTypeNode GetContextGraph(Type valueType)
        {
            lock (ContextCacheLock)
            {
                RootTypeNode graph;
                if (ContextCache.TryGetValue(valueType, out graph))
                    return graph;

                graph = new RootTypeNode(valueType);
                ContextCache.Add(valueType, graph);

                return graph;
            }
        }

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

                /* We have to dynamically generate a type graph for this new type */
                var contextGraph = GetContextGraph(value.GetType());
                var contextSerializer = (ContextValueNode)contextGraph.CreateSerializer(this);
                contextSerializer.Value = value;

                Children.AddRange(contextSerializer.Child.Children);
            }
        }

        public override Endianness Endianness
        {
            get { return EndiannessCallback(); }
        }

        public override Encoding Encoding
        {
            get { return EncodingCallback(); }
        }

        public Func<Endianness> EndiannessCallback { get; set; }

        public Func<Encoding> EncodingCallback { get; set; } 

        public override void Bind()
        {
            Child.Bind();
        }

        protected override void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            Child.Serialize(stream, eventShuttle);
        }

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            Child = ((RootTypeNode)TypeNode).Child.CreateSerializer(this);
            Child.Deserialize(stream, eventShuttle);
        }
    }
}

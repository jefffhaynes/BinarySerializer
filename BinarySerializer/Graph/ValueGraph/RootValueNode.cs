using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class RootValueNode : ValueNode
    {
        private static readonly Dictionary<Type, RootTypeNode> ContextCache = new Dictionary<Type, RootTypeNode>();
        private static readonly object ContextCacheLock = new object();

        public RootValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public ValueNode Child { get; private set; }

        public override object Value
        {
            get => Child?.Value;

            set
            {
                Child = ((RootTypeNode) TypeNode).Child.CreateSerializer(this);
                Child.Value = value;
            }
        }

        public override object BoundValue => Child.BoundValue;

        public object Context
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                /* We have to dynamically generate a type graph for this new type */
                var contextGraph = GetContextGraph(value.GetType());
                var contextSerializer = (RootValueNode) contextGraph.CreateSerializer(this);
                contextSerializer.EncodingCallback = EncodingCallback;
                contextSerializer.EndiannessCallback = EndiannessCallback;
                contextSerializer.Value = value;

                // root or context nodes aren't part of serialization so they're created already "visited"
                contextSerializer.Child.Visited = true;

                Children.AddRange(contextSerializer.Child.Children);
            }
        }

        public Func<Endianness> EndiannessCallback { get; set; }

        public Func<Encoding> EncodingCallback { get; set; }

        public override void Bind()
        {
            Child.Bind();
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            Child.Serialize(stream, eventShuttle);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            Child = ((RootTypeNode) TypeNode).Child.CreateSerializer(this);
            Child.Deserialize(stream, eventShuttle);
        }

        internal override Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            Child = ((RootTypeNode) TypeNode).Child.CreateSerializer(this);
            return Child.DeserializeAsync(stream, eventShuttle, cancellationToken);
        }

        protected override Endianness GetFieldEndianness()
        {
            return EndiannessCallback();
        }

        protected override Encoding GetFieldEncoding()
        {
            return EncodingCallback();
        }

        private static RootTypeNode GetContextGraph(Type valueType)
        {
            lock (ContextCacheLock)
            {
                RootTypeNode graph;
                if (ContextCache.TryGetValue(valueType, out graph))
                {
                    return graph;
                }

                graph = new RootTypeNode(valueType);
                ContextCache.Add(valueType, graph);

                return graph;
            }
        }
    }
}
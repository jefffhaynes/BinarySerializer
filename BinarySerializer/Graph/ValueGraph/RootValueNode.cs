using System;
using System.Diagnostics.CodeAnalysis;
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

        public RootValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
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

                // We have to dynamically generate a type graph for this new type
                var contextGraph = GetContextGraph(GetRuntimeType(value));
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

        public override void BindChecks()
        {
            Child.BindChecks();
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            Child.Serialize(stream, eventShuttle);
        }

        internal override Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            return Child.SerializeAsync(stream, eventShuttle, true, cancellationToken);
        }

        internal override void DeserializeOverride(BoundedStream stream, SerializationOptions options, EventShuttle eventShuttle)
        {
            Child = ((RootTypeNode) TypeNode).Child.CreateSerializer(this);
            Child.Deserialize(stream, options, eventShuttle);
        }

        internal override Task DeserializeOverrideAsync(BoundedStream stream, SerializationOptions options, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            Child = ((RootTypeNode) TypeNode).Child.CreateSerializer(this);
            return Child.DeserializeAsync(stream, options, eventShuttle, cancellationToken);
        }

        protected override Endianness GetFieldEndianness()
        {
            return EndiannessCallback();
        }

        protected override Encoding GetFieldEncoding()
        {
            return EncodingCallback();
        }


        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        [UnconditionalSuppressMessage("Trimming", "IL2073",
            Justification = "Runtime type discovery for the serialization context; users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        private static Type GetRuntimeType(object value)
        {
            return value.GetType();
        }

        private static RootTypeNode GetContextGraph([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type valueType)
        {
            lock (ContextCacheLock)
            {
                if (ContextCache.TryGetValue(valueType, out var graph))
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
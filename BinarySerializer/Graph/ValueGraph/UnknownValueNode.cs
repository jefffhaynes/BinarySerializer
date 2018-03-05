using System;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class UnknownValueNode : ValueNode
    {
        private object _cachedValue;

        public UnknownValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get => _cachedValue;

            set
            {
                if (value == null)
                {
                    return;
                }

                var valueType = value.GetType();

                if (valueType == typeof(object))
                {
                    throw new InvalidOperationException("Unable to serialize object.");
                }

                /* Create graph as if parent were creating it */
                var unknownTypeGraph = new RootTypeNode(TypeNode.Parent, valueType);
                var unknownSerializer = (RootValueNode) unknownTypeGraph.CreateSerializer(Parent);
                unknownSerializer.EndiannessCallback = GetFieldEndianness;
                unknownSerializer.EncodingCallback = GetFieldEncoding;
                unknownSerializer.Value = value;
                Children.Add(unknownSerializer.Child);

                _cachedValue = value;
            }
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            foreach (var child in Children)
            {
                child.Serialize(stream, eventShuttle);
            }
        }

        internal override async Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            foreach (var child in Children)
            {
                await child.SerializeAsync(stream, eventShuttle, true, cancellationToken).ConfigureAwait(false);
            }
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            throw new InvalidOperationException("Deserializing object fields not supported.");
        }

        internal override Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            DeserializeOverride(stream, eventShuttle);
            #if NETSTANDARD1_3
            return Task.CompletedTask;
            #else
            return Task.FromResult(true);
            #endif

        }
    }
}
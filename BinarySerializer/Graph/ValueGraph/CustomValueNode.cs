using System;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class CustomValueNode : ObjectValueNode
    {
        public CustomValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void ObjectSerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateLazySerializationContext();

            var value = BoundValue;

            if (value == null)
            {
                return;
            }

            if (!(value is IBinarySerializable binarySerializable))
            {
                throw new InvalidOperationException("Must implement IBinarySerializable");
            }

            binarySerializable.Serialize(stream, GetFieldEndianness(), serializationContext);
        }

        protected override void ObjectDeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateLazySerializationContext();
            var binarySerializable = CreateBinarySerializable();
            binarySerializable.Deserialize(stream, GetFieldEndianness(), serializationContext);
            Value = binarySerializable;
        }

        protected override Task ObjectDeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            ObjectDeserializeOverride(stream, eventShuttle);
            #if NETSTANDARD1_3
            return Task.CompletedTask;
            #else
            return Task.FromResult(true);
            #endif
        }

        protected override Task ObjectSerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            ObjectSerializeOverride(stream, eventShuttle);
            #if NETSTANDARD1_3
            return Task.CompletedTask;
            #else
            return Task.FromResult(true);
            #endif

        }

        private IBinarySerializable CreateBinarySerializable()
        {
            var binarySerializable = (IBinarySerializable) Activator.CreateInstance(TypeNode.Type);
            return binarySerializable;
        }
    }
}
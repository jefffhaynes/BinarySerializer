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

            var binarySerializable = value as IBinarySerializable;

            if (binarySerializable == null)
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
            return Task.CompletedTask;
        }

        private IBinarySerializable CreateBinarySerializable()
        {
            var binarySerializable = (IBinarySerializable) Activator.CreateInstance(TypeNode.Type);
            return binarySerializable;
        }
    }
}
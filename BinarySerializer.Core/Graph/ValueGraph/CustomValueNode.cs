using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class CustomValueNode : ObjectValueNode
    {
        public CustomValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void ObjectSerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();

            var value = BoundValue;

            if (value == null)
                return;

            var binarySerializable = value as IBinarySerializable;

            if (binarySerializable == null)
                throw new InvalidOperationException("Must implement IBinarySerializable");

            binarySerializable.Serialize(stream, Endianness, serializationContext);
        }

        protected override void ObjectDeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();
            var binarySerializable = (IBinarySerializable)Activator.CreateInstance(TypeNode.Type);
            binarySerializable.Deserialize(stream, Endianness, serializationContext);
            Value = binarySerializable;
        }
    }
}

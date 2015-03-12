using System;
using System.IO;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class CustomValueNode : ValueNode
    {
        private IBinarySerializable _binarySerializable;

        public CustomValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get { return _binarySerializable; }

            set
            {
                var binarySerializable = value as IBinarySerializable;

                if(binarySerializable == null)
                    throw new InvalidOperationException("Must implement IBinarySerializable");

                _binarySerializable = binarySerializable;
            }
        }

        protected override void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();
            _binarySerializable.Serialize(stream, Endianness, serializationContext);
        }

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();
            _binarySerializable = (IBinarySerializable)Activator.CreateInstance(TypeNode.Type);
            _binarySerializable.Deserialize(stream, Endianness, serializationContext);
        }
    }
}

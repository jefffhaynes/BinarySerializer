using System.IO;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class CustomValueNode : ValueNode
    {
        public CustomValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }
        protected override void SerializeOverride(Stream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();
            var value = (IBinarySerializable)Value;
            value.Serialize(stream, Endianness, serializationContext);
        }

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();
            var value = (IBinarySerializable)Value;
            value.Deserialize(stream, Endianness, serializationContext);
        }
    }
}

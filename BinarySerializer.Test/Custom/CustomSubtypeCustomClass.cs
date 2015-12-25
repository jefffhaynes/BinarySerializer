using System.IO;

namespace BinarySerialization.Test.Custom
{
    public class CustomSubtypeCustomClass : CustomSubtypeBaseClass, IBinarySerializable
    {
        [Ignore]
        public uint Value { get; set; }

        public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
            var varuint = new Varuint {Value = Value};
            varuint.Serialize(stream, endianness, serializationContext);
        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
            var varuint = new Varuint { Value = Value };
            varuint.Deserialize(stream, endianness, serializationContext);
            Value = varuint.Value;
        }
    }
}

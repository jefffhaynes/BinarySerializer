using System.IO;
using Xunit;

namespace BinarySerialization.Test.Custom
{
    public class CustomSubtypeCustomClass : CustomSubtypeBaseClass, IBinarySerializable
    {
        [Ignore]
        public uint Value { get; set; }

        public void Serialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            var boundedStream = (BoundedStream) stream;
            Assert.Equal(0, boundedStream.Position);
            Assert.Equal(100, (int) boundedStream.MaxLength.ByteCount);

            var varuint = new Varuint {Value = Value};
            varuint.Serialize(stream, endianness, serializationContext);
        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            var varuint = new Varuint {Value = Value};
            varuint.Deserialize(stream, endianness, serializationContext);
            Value = varuint.Value;
        }
    }
}
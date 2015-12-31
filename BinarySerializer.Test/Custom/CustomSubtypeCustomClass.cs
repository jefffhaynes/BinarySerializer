using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Custom
{
    public class CustomSubtypeCustomClass : CustomSubtypeBaseClass, IBinarySerializable
    {
        [Ignore]
        public uint Value { get; set; }

        public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
            var limitedStream = (LimitedStream) stream;
            Assert.AreEqual(0, limitedStream.Position);
            Assert.AreEqual(100, limitedStream.MaxLength);

            var varuint = new Varuint {Value = Value};
            varuint.Serialize(stream, endianness, serializationContext);
        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
            Assert.AreEqual(100, stream.Length);
            var varuint = new Varuint { Value = Value };
            varuint.Deserialize(stream, endianness, serializationContext);
            Value = varuint.Value;
        }
    }
}

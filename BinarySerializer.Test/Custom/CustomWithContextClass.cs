using System.IO;

namespace BinarySerialization.Test.Custom
{
    public class CustomWithContextClass : IBinarySerializable
    {
        public void Serialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            // TODO check context
        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
        }
    }
}
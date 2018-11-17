using System.IO;
using System.Linq;

namespace BinarySerialization.Test.ItemSubtype
{
    public class CustomItem : IItemSubtype, IBinarySerializable
    {
        public static readonly byte[] Data = System.Text.Encoding.ASCII.GetBytes("hello");

        public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
            stream.Write(Data, 0, Data.Length);
        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
            var data = new byte[Data.Length];
            stream.Read(data, 0, data.Length);

            if(!data.SequenceEqual(Data))
                throw new InvalidDataException();
        }
    }
}

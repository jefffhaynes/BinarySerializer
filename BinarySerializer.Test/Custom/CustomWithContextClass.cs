using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace BinarySerializer.Test.Custom
{
    public class CustomWithContextClass : IBinarySerializable
    {
        public object Context { get; set; }

        public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {

        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
        {
        }
    }
}

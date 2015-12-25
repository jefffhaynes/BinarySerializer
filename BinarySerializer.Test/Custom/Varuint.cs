using System;
using System.IO;

namespace BinarySerialization.Test.Custom
{
    /// <summary>
    /// A class for multibyte representation of an integer.
    /// </summary>
    public class Varuint : IBinarySerializable
    {
        [Ignore]
        public uint Value { get; set; }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext context)
        {
            bool more = true;
            int shift = 0;

            Value = 0;

            while (more)
            {
                int b = stream.ReadByte();

                if (b == -1)
                    throw new InvalidOperationException("Reached end of stream before end of varuint.");

                var lower7Bits = (byte)b;
                more = (lower7Bits & 128) != 0;
                Value |= (uint)((lower7Bits & 0x7f) << shift);
                shift += 7;
            }
        }

        public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext context)
        {
            var value = Value;
            do
            {
                var lower7Bits = (byte) (value & 0x7f);
                value >>= 7;
                if (value > 0)
                    lower7Bits |= 128;
                stream.WriteByte(lower7Bits);
            } while (value > 0);
        }
    }
}

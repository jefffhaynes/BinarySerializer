using BinarySerialization;

namespace BinarySerializer.Editor.Test
{
    public class PngChunkContainer
    {
        [FieldOrder(0)]
        [FieldEndianness(Endianness.Big)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldCrc32("Crc", Polynomial = 0x04c11db7)]
        [FieldEndianness(Endianness.Big)]
        public PngChunkPayload Payload { get; set; }

        [FieldOrder(2)]
        [FieldEndianness(Endianness.Big)]
        public uint Crc { get; set; }
    }
}

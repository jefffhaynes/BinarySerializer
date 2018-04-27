using System.IO;

namespace BinarySerialization.Test.Value
{
    public class StreamValueClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length))]
        [FieldCrc16(nameof(Crc))]
        public Stream Data { get; set; }

        [FieldOrder(2)]
        public ushort Crc { get; set; }
    }
}
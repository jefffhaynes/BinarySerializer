namespace BinarySerialization.Test.Issues.Issue78
{
    public class CrcWrap
    {
        [FieldOrder(1)]
        [FieldCrc16(nameof(Crc), Polynomial = 0x1021, InitialValue = 0xffff)]
        public Frame Frame { get; set; }
        [FieldOrder(2)]
        [FieldEndianness(BinarySerialization.Endianness.Big)]
        public ushort Crc { get; set; }
    }
}

namespace BinarySerialization.Test.BitLength
{
    public class BitLengthValueClass
    {
        [FieldOrder(0)]
        [FieldCrc16("Crc")]
        public InternalBitLengthValueClass Value { get; set; }

        [FieldOrder(1)]
        public ushort Crc { get; set; }

        [FieldOrder(2)]
        [FieldCrc16("Crc2")]
        public byte Value2 { get; set; }

        [FieldOrder(3)]
        public ushort Crc2 { get; set; }
    }
}

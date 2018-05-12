namespace BinarySerialization.Test.Value
{
    public class NestedCrcClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length))]
        [FieldCrc16(nameof(Crc))]
        public string Value { get; set; }

        [FieldOrder(2)]
        public ushort Crc { get; set; }
    }
}

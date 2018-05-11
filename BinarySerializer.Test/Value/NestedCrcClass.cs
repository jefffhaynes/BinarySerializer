namespace BinarySerialization.Test.Value
{
    public class NestedCrcClass
    {
        [FieldOrder(0)]
        [FieldCrc16(nameof(Crc))]
        public int Value { get; set; }

        [FieldOrder(1)]
        public ushort Crc { get; set; }
    }
}

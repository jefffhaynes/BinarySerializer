namespace BinarySerialization.Test.Value
{
    public class ValueClass
    {
        [FieldOrder(0)]
        [FieldCrc16("Crc")]
        public ValueInternalClass Internal { get; set; }

        [FieldOrder(1)]
        public int Crc { get; set; }
    }
}

namespace BinarySerialization.Test.Value
{
    public class FieldCrc16MultiFieldClass
    {
        [FieldOrder(0)]
        [FieldCrc16("Crc")]
        public byte Value1 { get; set; }

        [FieldOrder(1)]
        [FieldCrc16("Crc2")]
        public ushort Value2 { get; set; }
  
        [FieldOrder(2)]
        [FieldCrc16("Crc")]
        public byte Value3 { get; set; }

        [FieldOrder(3)]
        public ushort Crc { get; set; }

        [FieldOrder(4)]
        public ushort Crc2 { get; set; }
    }
}

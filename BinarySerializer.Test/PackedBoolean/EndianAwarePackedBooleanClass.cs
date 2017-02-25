using BinarySerialization;

namespace BinarySerialization.Test.PackedBoolean
{
    public class EndianAwarePackedBooleanClass
    {
        [FieldEndianness(BinarySerialization.Endianness.Little)]
        [FieldOrder(0)]
        public bool[] LittleEndianArray { get; set; }

        [FieldEndianness(BinarySerialization.Endianness.Big)]
        [FieldOrder(1)]
        public bool[] BigEndianArray { get; set; }        
    }
}

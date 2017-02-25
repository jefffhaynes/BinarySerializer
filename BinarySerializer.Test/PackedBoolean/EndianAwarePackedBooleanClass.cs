using BinarySerialization;

namespace BinarySerialization.Test.PackedBoolean
{
    public class EndianAwarePackedBooleanClass
    {
        [FieldEndianness(BinarySerialization.Endianness.Little)]
        [FieldOrder(0), Pack]
        public bool[] LittleEndianArray { get; set; }

        [FieldEndianness(BinarySerialization.Endianness.Big)]
        [FieldOrder(1), Pack]
        public bool[] BigEndianArray { get; set; }        
    }
}

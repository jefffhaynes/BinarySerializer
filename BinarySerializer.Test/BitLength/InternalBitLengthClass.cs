namespace BinarySerialization.Test.BitLength
{
    public class InternalBitLengthClass
    {
        [FieldOrder(0)]
        [FieldBitLength(4)]
        public int Value { get; set; }
    }
}

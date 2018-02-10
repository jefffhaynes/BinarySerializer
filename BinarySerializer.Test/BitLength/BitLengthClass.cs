namespace BinarySerialization.Test.BitLength
{
    public class BitLengthClass
    {
        [FieldOrder(0)]
        [FieldBitLength(2)]
        public int A { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(6)]
        public int B { get; set; }
    }
}

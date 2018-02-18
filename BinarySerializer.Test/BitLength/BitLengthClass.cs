namespace BinarySerialization.Test.BitLength
{
    public class BitLengthClass
    {
        [FieldOrder(0)]
        [FieldBitLength(21)]
        public int A { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(3)]
        public int B { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(8)]
        public int C { get; set; }
    }
}

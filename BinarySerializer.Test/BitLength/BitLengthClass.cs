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
        [FieldBitLength(4)]
        public int C { get; set; }

        [FieldOrder(3)]
        public InternalBitLengthClass Internal { get; set; }

        [FieldOrder(4)]
        [FieldBitLength(8)]
        public InternalBitLengthClass Internal2 { get; set; }
    }
}

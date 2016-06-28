namespace BinarySerialization.Test.ItemLength
{
    public class JaggedIntArrayClass
    {
        [FieldOrder(0)]
        public int ArrayCount { get; set; }

        [FieldOrder(1)]
        [FieldCount("ArrayCount")]
        public byte[] ArrayLengths { get; set; }

        [FieldOrder(2)]
        [FieldCount("ArrayCount")]
        [ItemLength("ArrayLengths")]
        public int[][] Arrays { get; set; }
    }
}
namespace BinarySerialization.Test.ItemLength
{
    public class JaggedIntArrayClass
    {
        [FieldOrder(0)]
        public int ArrayCount { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(ArrayCount))]
        public byte[] ArrayLengths { get; set; }

        [FieldOrder(2)]
        [FieldCount(nameof(ArrayCount))]
        [ItemLength(nameof(ArrayLengths))]
        public int[][] Arrays { get; set; }
    }
}
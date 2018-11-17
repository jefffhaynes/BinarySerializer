namespace BinarySerialization.Test.Issues.Issue18
{
    public class JaggedArrayClass
    {
        [FieldOrder(0)]
        public int ArrayCount { get; set; }

        [FieldOrder(1)]
        [FieldCount("ArrayCount")]
        public int[] ArrayLengths { get; set; }

        [FieldOrder(2)]
        [FieldCount("ArrayCount")]
        [ItemLength("ArrayLengths")]
        public byte[][] Arrays { get; set; }
    }
}
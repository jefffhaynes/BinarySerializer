namespace BinarySerialization.Test.Issues.Issue106
{
    public class Buffer
    {
        [FieldOrder(0)]
        [FieldBitLength(2)]
        public byte Field1 { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(7)]
        public byte Field2 { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(7)]
        public byte Field3 { get; set; }

        [FieldOrder(3)]
        public byte Field4 { get; set; }
    }
}

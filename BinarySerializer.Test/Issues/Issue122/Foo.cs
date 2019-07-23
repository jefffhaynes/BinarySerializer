namespace BinarySerialization.Test.Issues.Issue122
{
    public class Foo
    {
        [FieldOrder(0)]
        [FieldBitLength(4)]
        public byte X { get; set; } = 0x01;

        [FieldOrder(1)]
        [FieldBitLength(4)]
        public byte Y { get; set; } = 0x02;
    }
}

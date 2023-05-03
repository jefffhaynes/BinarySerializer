namespace BinarySerialization.Test.Issues.Issue205
{
    public class Segment
    {
        [FieldOrder(0)]
        [FieldBitLength(3)]
        public byte Type { get; set; }

        [FieldOrder(1)]
        public PortSegment PortSegment { get; set; }
    }

    public class PortSegment
    {
        [FieldOrder(0)]
        [FieldBitLength(1)]
        public bool ExtendedLinkAddress { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(4)]
        public byte Port { get; set; }

        [FieldOrder(2)]
        public byte LinkAddress;
    }
}

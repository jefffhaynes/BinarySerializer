namespace BinarySerialization.Test.Issues.Issue148
{
    public class ProtocolClass
    {
        [FieldOrder(1)]
        public HeaderClass Header { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(4)]
        public byte Body { get; set; }

    }
}
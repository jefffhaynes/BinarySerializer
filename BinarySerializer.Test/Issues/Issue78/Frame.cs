namespace BinarySerialization.Test.Issues.Issue78
{
    public class Frame
    {
        [FieldOrder(1)]
        public uint Length { get; set; }

        [FieldOrder(2)]
        public uint MagicNumber { get; set; }

        [FieldOrder(3)]
        [FieldLength(nameof(Length))]
        public Payload Payload { get; set; }
    }
}
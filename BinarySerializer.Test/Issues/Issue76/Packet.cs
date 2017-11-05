namespace BinarySerialization.Test.Issues.Issue76
{
    public class Packet
    {
        [FieldOrder(0)]
        public uint Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public PacketContent Content { get; set; }
    }
}
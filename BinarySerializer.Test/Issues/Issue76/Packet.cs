namespace BinarySerialization.Test.Issues.Issue76
{
    public class Packet
    {
        [FieldOrder(0)]
        public uint Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        [FieldChecksum("Checksum", Mode = ChecksumMode.Xor)]
        public PacketContent Content { get; set; }

        [FieldOrder(2)]
        public byte Checksum { get; set; }
    }
}
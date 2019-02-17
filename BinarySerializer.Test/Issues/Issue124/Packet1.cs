namespace BinarySerialization.Test.Issues.Issue124
{
    public class Packet1 : PacketBody
    {
        [FieldBitLength(4)]
        public int Value { get; set; }
    }
}
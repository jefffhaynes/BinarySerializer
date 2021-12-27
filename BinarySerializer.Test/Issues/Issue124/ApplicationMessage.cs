namespace BinarySerialization.Test.Issues.Issue124;

public class ApplicationMessage
{
    [FieldOrder(1)]
    [FieldBitLength(10)]
    public byte NID_STM { get; set; }


    [FieldOrder(2)]
    [FieldBitLength(13)]
    public ushort L_MESSAGE { get; set; }

    [FieldOrder(3)]
    // L_MESSAGE = (bit length of List<Packet> + bit length of NID_STM(10) + bit length of L_MESSAGE(13) + bit length  of PaddingBits ) /8
    [FieldBitLength(nameof(L_MESSAGE))]
    [ItemSerializeUntil(nameof(Packet.NID_PACKET), null)]
    public List<Packet> Packets;

    //PaddingBits bit length = L_MESSAGE * 8 - bit length of List<Packet> - bit length of NID_STM(10) - bit length of L_MESSAGE(13)
    [FieldOrder(4)]
    public byte PaddingBits { get; set; }
}

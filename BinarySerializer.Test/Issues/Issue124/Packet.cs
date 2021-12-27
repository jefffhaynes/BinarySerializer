namespace BinarySerialization.Test.Issues.Issue124;

public class Packet
{

    [FieldOrder(1)]
    [FieldBitLength(10)]
    public byte NID_PACKET { get; set; }

    [FieldOrder(2)]
    [FieldBitLength(13)]
    public ushort L_PACKET { get; set; }

    [FieldOrder(3)]
    [SerializeWhenNot(nameof(NID_PACKET), null)]
    [Subtype(nameof(NID_PACKET), NID_PACKETNUM.PACKET1, typeof(Packet1))]
    [Subtype(nameof(NID_PACKET), NID_PACKETNUM.PACKETN, typeof(PacketN))]
    //L_PACKET = bit length of PacketBody + bit length of NID_PACKET(10)+ bit length of L_PACKET(13)
    [FieldBitLength(nameof(L_PACKET))]
    public PacketBody PacketBody { get; set; }

}

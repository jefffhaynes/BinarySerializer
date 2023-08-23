namespace BinarySerialization.Test.Issues.Issue124
{
    public class Packet
    {

        [FieldOrder(1)]
        [FieldBitLength(10)]
        public byte NID_PACKET { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(13)]
        public ushort L_PACKET { get; set; }

        [FieldOrder(3)]
#pragma warning disable CS0618 // Type or member is obsolete
        [SerializeWhenNot(nameof(NID_PACKET), null)]
#pragma warning restore CS0618 // Type or member is obsolete
        [Subtype(nameof(NID_PACKET), NID_PACKETNUM.PACKET1, typeof(Packet1), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(NID_PACKET), NID_PACKETNUM.PACKETN, typeof(PacketN), BindingMode = BindingMode.OneWay)]
        //L_PACKET = bit length of PacketBody + bit length of NID_PACKET(10)+ bit length of L_PACKET(13)
        [FieldBitLength(nameof(L_PACKET))]
        public PacketBody PacketBody { get; set; }

    }
}
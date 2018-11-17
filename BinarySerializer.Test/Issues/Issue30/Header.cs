namespace BinarySerialization.Test.Issues.Issue30
{
    public class Header
    {
        public Header()
        {
            Stx = 2;
            HeaderDef = 1;
            Behaviour = 3;
            Security = 0;
        }

        public Header(ushort sender, ushort receiver, MessageType msgType, uint telegrammId)
            : this(sender, receiver, msgType, telegrammId, 0, PayloadType.UnDefined)
        {
        }

        public Header(ushort sender, ushort receiver, MessageType msgType, uint telegrammId, uint size, PayloadType type)
            : this()
        {
            SenderId = sender;
            ReceiverId = receiver;
            MsgType = msgType;
            TelegrammId = telegrammId;
            PayloadSize = size;
            PayloadType = type;
        }

        [FieldOrder(0)]
        public byte Stx { get; set; }

        [FieldOrder(1)]
        public byte HeaderDef { get; set; }

        [FieldOrder(2)]
        public byte Behaviour { get; set; }

        [FieldOrder(3)]
        public byte Security { get; set; }

        [FieldOrder(4)]
        public ushort SenderId { get; set; }

        [FieldOrder(5)]
        public ushort ReceiverId { get; set; }

        [FieldOrder(6)]
        public MessageType MsgType { get; set; }

        [FieldOrder(7)]
        public uint TelegrammId { get; set; }

        [FieldOrder(8)]
        public uint PayloadSize { get; set; }

        [FieldOrder(9)]
        public PayloadType PayloadType { get; set; }
    }
}
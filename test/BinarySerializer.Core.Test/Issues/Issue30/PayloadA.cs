namespace BinarySerialization.Test.Issues.Issue30
{
    public class PayloadA : IPayload
    {
        [FieldOrder(0)]
        public byte Mode { get; set; }

        [FieldOrder(1)]
        public byte Cnt { get; set; }

        public PayloadA()
        {
        }

        public PayloadA(byte mode, byte cnt)
        {
            Mode = mode;
            Cnt = cnt;
        }

        public PayloadType GetPayloadType()
        {
            return PayloadType.STAMR;
        }

        public uint GetPayloadLength()
        {
            return 100;
        }
    }
}

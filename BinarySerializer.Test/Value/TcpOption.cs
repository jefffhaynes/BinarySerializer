namespace BinarySerialization.Test.Value
{
    public class TcpOption
    {
        [FieldOrder(0)]
        public TcpOptionKind Kind { get; set; }

        [FieldOrder(1)]
        [SerializeWhenNot(nameof(Kind), TcpOptionKind.End)]
        [SerializeWhenNot(nameof(Kind), TcpOptionKind.NoOp)]
        public byte Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(nameof(Length))]
        [SerializeWhenNot(nameof(Kind), TcpOptionKind.End)]
        [SerializeWhenNot(nameof(Kind), TcpOptionKind.NoOp)]
        public byte[] Option { get; set; }
    }
}

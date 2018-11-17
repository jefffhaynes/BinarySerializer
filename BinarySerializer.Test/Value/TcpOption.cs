namespace BinarySerialization.Test.Value
{
    public class TcpOption
    {
        [FieldOrder(0)]
        public TcpOptionKind Kind { get; set; }

        [FieldOrder(1)]
        [SerializeWhenNot("Kind", TcpOptionKind.End)]
        [SerializeWhenNot("Kind", TcpOptionKind.NoOp)]
        public byte Length { get; set; }

        [FieldOrder(2)]
        [FieldLength("Length")]
        [SerializeWhenNot("Kind", TcpOptionKind.End)]
        [SerializeWhenNot("Kind", TcpOptionKind.NoOp)]
        public byte[] Option { get; set; }
    }
}

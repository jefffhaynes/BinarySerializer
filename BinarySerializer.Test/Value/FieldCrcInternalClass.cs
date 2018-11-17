namespace BinarySerialization.Test.Value
{
    public class FieldCrcInternalClass
    {
        [FieldOrder(0)]
        public ushort UshortValue { get; set; }

        [FieldOrder(1)]
        public byte ByteValue { get; set; }

        [FieldOrder(2)]
        [FieldLength(2)]
        public byte[] ArrayValue { get; set; }

        [FieldOrder(3)]
        [SerializeAs(SerializedType.SizedString)]
        public string Value { get; set; }
    }
}
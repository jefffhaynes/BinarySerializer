namespace BinarySerialization.Test.Value
{
    public class FieldCrcInternalClass
    {
        [FieldOrder(0)]
        public byte IntegerValue { get; set; }

        [FieldOrder(1)]
        [SerializeAs(SerializedType.SizedString)]
        public string Value { get; set; }
    }
}
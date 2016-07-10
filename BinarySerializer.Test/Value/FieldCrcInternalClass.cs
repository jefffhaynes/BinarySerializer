namespace BinarySerialization.Test.Value
{
    public class FieldCrcInternalClass
    {
        [FieldOrder(0)]
        [SerializeAs(SerializedType.SizedString)]
        public string Value { get; set; }
    }
}
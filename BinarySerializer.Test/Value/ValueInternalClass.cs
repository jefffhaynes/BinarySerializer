namespace BinarySerialization.Test.Value
{
    public class ValueInternalClass
    {
        [FieldOrder(0)]
        [SerializeAs(SerializedType.SizedString)]
        public string Value { get; set; }
    }
}

namespace BinarySerialization.Test.Enums
{
    public class ByteEnumArrayClass
    {
        [FieldOrder(0)]
        [FieldCount(3)]
        public BaseTypeEnumValues[] Field { get; set; }
    }
}

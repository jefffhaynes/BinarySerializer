namespace BinarySerialization.Test.Enums
{
    internal class BaseTypeEnumAsStringClass
    {
        [SerializeAs(SerializedType.NullTerminatedString)]
        public BaseTypeEnumValues Field { get; set; }
    }
}
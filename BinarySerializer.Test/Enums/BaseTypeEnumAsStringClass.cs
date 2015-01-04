using BinarySerialization;

namespace BinarySerializer.Test.Enums
{
    class BaseTypeEnumAsStringClass
    {
        [SerializeAs(SerializedType.NullTerminatedString)]
        public BaseTypeEnumValues Field { get; set; }
    }
}

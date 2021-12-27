namespace BinarySerialization.Test.Enums;

internal class BaseTypeEnumAsStringClass
{
#pragma warning disable 618
    [SerializeAs(SerializedType.NullTerminatedString)]
#pragma warning restore 618
    public BaseTypeEnumValues Field { get; set; }
}

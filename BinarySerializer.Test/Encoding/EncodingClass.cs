namespace BinarySerialization.Test.Encoding;

public class EncodingClass
{
    [FieldEncoding("windows-1256")]
#pragma warning disable 618
    [SerializeAs(SerializedType.NullTerminatedString)]
#pragma warning restore 618
    public string Name { get; set; }
}

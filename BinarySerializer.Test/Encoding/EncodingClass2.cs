namespace BinarySerialization.Test.Encoding;

public class EncodingClass2
{
    [FieldEncoding("windows-1256")]
    [SerializeAs(SerializedType.TerminatedString)]
    public string Name { get; set; }
}

public class EncodingClassUtf16
{
    [FieldEncoding("UTF-16")]
    [SerializeAs(SerializedType.TerminatedString)]
    public string Name { get; set; }
}

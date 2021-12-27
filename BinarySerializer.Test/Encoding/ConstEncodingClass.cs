namespace BinarySerialization.Test.Encoding;

public class ConstEncodingClass
{
    [FieldEncoding("windows-1256")]
    public string Value { get; set; }
}

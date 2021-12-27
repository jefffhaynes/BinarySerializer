namespace BinarySerialization.Test.Issues.Issue9;

public class Entry
{
    [FieldOrder(0)]
    public byte Length { get; set; }

    [FieldOrder(1)]
    [FieldLength("Length")]
    public string Value { get; set; }
}

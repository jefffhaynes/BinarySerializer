namespace BinarySerialization.Test.Issues.Issue61;

public class Message
{
    [FieldOrder(0)]
    public int Length { get; set; }

    [FieldOrder(1)]
    public byte[] Data { get; set; }
}

namespace BinarySerialization.Test.Issues.Issue151;

public class GenericNACKPayload : dPayload
{
    [FieldOrder(0)]
    [FieldLength(1)]
    public byte NackCode { get; set; }
}

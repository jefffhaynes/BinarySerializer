namespace BinarySerialization.Test.Alignment;

public class BoundAlignmentClass
{
    [FieldOrder(0)]
    public byte Length { get; set; }

    [FieldOrder(1)]
    public byte Alignment { get; set; }

    [FieldOrder(2)]
    [FieldLength(nameof(Length))]
    [FieldAlignment(nameof(Alignment))]
    public string Value { get; set; }
}

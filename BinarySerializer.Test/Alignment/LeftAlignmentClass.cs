namespace BinarySerialization.Test.Alignment;

public class LeftAlignmentClass
{
    [FieldOrder(0)]
    public byte Header { get; set; }

    [FieldOrder(1)]
    [FieldAlignment(4, FieldAlignmentMode.LeftOnly)]
    public byte Value { get; set; }

    [FieldOrder(2)]
    public byte Trailer { get; set; }
}

namespace BinarySerialization.Test.Issues.Issue107;

public class ClassToSerialize
{
    [FieldOrder(0)]
    public byte Field1 { get; set; }
    [FieldOrder(1)]
    [FieldBitLength(4)]
    public byte Field2 { get; set; }
    [FieldOrder(2)]
    [FieldBitLength(6)]
    public byte Field3 { get; set; }
    [FieldOrder(3)]
    public byte Field4 { get; set; }
    [FieldOrder(4)]
    [FieldBitLength(6)]
    public byte Field5 { get; set; }
}

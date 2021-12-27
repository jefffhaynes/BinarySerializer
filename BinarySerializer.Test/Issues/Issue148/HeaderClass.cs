namespace BinarySerialization.Test.Issues.Issue148;

public class HeaderClass
{
    [FieldOrder(1)]
    [FieldBitLength(1)]
    public byte ITEM1 { get; set; }

    [FieldOrder(2)]
    [FieldBitLength(7)]
    public byte ITEM2 { get; set; }

    [FieldOrder(3)]
    [FieldBitLength(1)]
    public byte ITEM3 { get; set; }

    [FieldOrder(4)]
    [FieldBitLength(3)]
    public byte ITEM4 { get; set; }

}

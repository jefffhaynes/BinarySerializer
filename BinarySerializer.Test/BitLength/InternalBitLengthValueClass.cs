namespace BinarySerialization.Test.BitLength;

public class InternalBitLengthValueClass
{
    [FieldOrder(0)]
    [FieldBitLength(4)]
    public int Value { get; set; }

    [FieldOrder(1)]
    [FieldBitLength(4)]
    public int Value2 { get; set; }
}

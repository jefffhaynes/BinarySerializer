namespace BinarySerialization.Test.Issues.Issue122;

public class LaserFrame
{
    //[FieldEndianness(BinarySerialization.Endianness.Little)]
    [FieldOrder(0)]
    [FieldBitLength(12)]
    public uint X { get; set; }

    //[FieldEndianness(BinarySerialization.Endianness.Little)]
    [FieldOrder(1)]
    [FieldBitLength(12)]
    public uint Y { get; set; }

    [FieldOrder(2)] public byte R { get; set; }
    [FieldOrder(3)] public byte G { get; set; }
    [FieldOrder(4)] public byte B { get; set; }
}

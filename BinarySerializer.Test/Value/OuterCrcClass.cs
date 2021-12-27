namespace BinarySerialization.Test.Value;

public class OuterCrcClass
{
    [FieldOrder(0)]
    [FieldCrc16(nameof(Crc))]
    public NestedCrcClass NestedCrc { get; set; }

    [FieldOrder(1)]
    public ushort Crc { get; set; }
}

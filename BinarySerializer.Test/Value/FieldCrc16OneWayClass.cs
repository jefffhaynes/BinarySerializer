namespace BinarySerialization.Test.Value;

public class FieldCrc16OneWayClass
{
    [FieldOrder(0)]
    public int Length { get; set; }

    [FieldOrder(1)]
    [FieldLength(nameof(Length))]
    [FieldCrc16(nameof(Crc), BindingMode = BindingMode.OneWay)]
    public FieldCrcInternalClass Internal { get; set; }

    [FieldOrder(2)]
    public ushort Crc { get; set; }
}

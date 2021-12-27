namespace BinarySerialization.Test.Issues.Issue103;

public class MultipleFieldsCrc32
{
    [FieldOrder(1)]
    [FieldEndianness(BinarySerialization.Endianness.Big)]
    [FieldCrc32(nameof(Crc32), Polynomial = 0x04C10DB7, InitialValue = 0, IsDataReflected = false, IsRemainderReflected = false, FinalXor = 0)]
    public ushort Length { get; set; }

    [FieldOrder(3)]
    [FieldCrc32(nameof(Crc32), Polynomial = 0x04C10DB7, InitialValue = 0, IsDataReflected = false, IsRemainderReflected = false, FinalXor = 0)]
    [FieldLength(nameof(Length))]
    public string Msgs { get; set; }

    [FieldOrder(4)]
    public uint Crc32 { get; set; }
}

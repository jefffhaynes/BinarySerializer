namespace BinarySerialization.Test.Endianness;

public class FieldEndiannessClass
{
    [FieldOrder(0)]
    public uint Endianness { get; set; }

    [FieldOrder(1)]
    [FieldEndianness(nameof(Endianness), typeof(EndiannessConverter))]
    public ushort Value { get; set; }
}

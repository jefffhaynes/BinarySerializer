namespace BinarySerialization.Test.Endianness;

public class FieldEndiannessInvalidConverterClass
{
    [FieldOrder(0)]
    public int Endianness { get; set; }

    [FieldOrder(1)]
    [FieldEndianness("Endianness", typeof(InvalidEndiannessConverter))]
    public ushort Value { get; set; }
}

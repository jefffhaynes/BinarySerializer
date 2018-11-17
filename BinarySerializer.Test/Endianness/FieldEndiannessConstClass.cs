namespace BinarySerialization.Test.Endianness
{
    public class FieldEndiannessConstClass
    {
        [FieldEndianness(BinarySerialization.Endianness.Big)]
        public int Value { get; set; }
    }
}

namespace BinarySerialization.Test.Value
{
    public class FieldChecksumClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        [FieldChecksum("Checksum")]
        [FieldChecksum("ModuloChecksum", Mode = ChecksumMode.Modulo256)]
        [FieldChecksum("XorChecksum", Mode = ChecksumMode.Xor)]
        public string Value { get; set; }

        [FieldOrder(2)]
        public byte Checksum { get; set; }
        
        [FieldOrder(3)]
        public byte ModuloChecksum { get; set; }

        [FieldOrder(4)]
        public byte XorChecksum { get; set; }
    }
}

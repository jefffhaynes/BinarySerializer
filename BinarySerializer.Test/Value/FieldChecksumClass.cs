namespace BinarySerialization.Test.Value
{
    public class FieldChecksumClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        [FieldChecksum("Checksum")]
        public string Value { get; set; }

        [FieldOrder(2)]
        public byte Checksum { get; set; }
        
        [FieldOrder(3)]
        [FieldLength("Length")]
        [FieldChecksum("ModuloChecksum", Mode = ChecksumMode.Modulo256)]
        public string Value2 { get; set; }

        [FieldOrder(4)]
        public byte ModuloChecksum { get; set; }
        
        [FieldOrder(5)]
        [FieldLength("Length")]
        [FieldChecksum("XorChecksum", Mode = ChecksumMode.Xor)]
        public string Value3 { get; set; }

        [FieldOrder(6)]
        public byte XorChecksum { get; set; }
    }
}

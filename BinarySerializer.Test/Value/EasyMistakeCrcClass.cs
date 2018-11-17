namespace BinarySerialization.Test.Value
{
    public class EasyMistakeCrcClass
    {
        [FieldOrder(0)]
        public uint Value { get; set; }

        [FieldOrder(1)]
        [FieldCrc32("Value")]
        public uint Crc { get; set; }
    }
}

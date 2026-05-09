namespace BinarySerialization.Test.Offset
{
    public class ConstOffsetClass
    {
        [FieldOrder(0)]
        public uint FieldStringLength { get; set; }

        [FieldOrder(1)]
        [FieldOffset(100)]
        [FieldLength(nameof(FieldStringLength))]
        public string FieldString { get; set; }

        [FieldOrder(2)]
        public uint FieldUint { get; set; }
    }
}
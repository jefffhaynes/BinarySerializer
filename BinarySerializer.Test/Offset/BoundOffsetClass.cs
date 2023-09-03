namespace BinarySerialization.Test.Offset
{
    public class BoundOffsetClass
    {
        [FieldOrder(0)]
        public uint FieldOffsetField { get; set; }

        [FieldOrder(1)]
        [FieldOffset(nameof(FieldOffsetField))]
        public string FieldString { get; set; }

        [FieldOrder(2)]
        public uint FieldUint { get; set; }
    }
}
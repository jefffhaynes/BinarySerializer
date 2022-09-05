namespace BinarySerialization.Test.Position
{
    public class BoundPositionBeginClass
    {
        [FieldOrder(0)]
        public uint FieldOffsetField { get; set; }

        [FieldOrder(1)]
        [FieldPosition(nameof(FieldOffsetField))]
        public uint Field { get; set; }

        [FieldOrder(2)]
        public uint LastUInt { get; set; }

        [FieldOrder(3)]
        [FieldPosition(0)]
        public uint RepeatFieldOffsetField { get; set; }
    }
}
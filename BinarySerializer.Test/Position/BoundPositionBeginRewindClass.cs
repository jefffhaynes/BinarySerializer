namespace BinarySerialization.Test.Position
{
    public class BoundPositionBeginRewindClass
    {
        [FieldOrder(0)]
        public uint FieldOffsetField { get; set; }

        [FieldOrder(1)]
        [FieldPosition(nameof(FieldOffsetField), true)]
        public uint Field { get; set; }

        [FieldOrder(2)]
        public uint SecondUint { get; set; }
    }
}
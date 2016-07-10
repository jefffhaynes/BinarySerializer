namespace BinarySerialization.Test.Offset
{
    public class BoundOffsetClass
    {
        [FieldOrder(0)]
        public int FieldOffsetField { get; set; }

        [FieldOrder(1)]
        [FieldOffset("FieldOffsetField")]
        public string Field { get; set; }
    }
}
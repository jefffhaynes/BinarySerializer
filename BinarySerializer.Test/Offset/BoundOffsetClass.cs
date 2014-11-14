using BinarySerialization;

namespace BinarySerializer.Test.Offset
{
    public class BoundOffsetClass
    {
        public int FieldOffsetField { get; set; }

        [FieldOffset("FieldOffsetField")]
        public string Field { get; set; }
    }
}
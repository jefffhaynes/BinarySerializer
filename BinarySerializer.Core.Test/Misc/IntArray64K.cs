using BinarySerialization;

namespace BinarySerialization.Test.Misc
{
    public class IntArray64K
    {
        [FieldOrder(0)]
        [FieldLength(65536 * sizeof(int))]
        public int[] Array { get; set; }

        [FieldOrder(1)]
        [FieldCount(65536)]
        public int[] Array2 { get; set; }
    }
}

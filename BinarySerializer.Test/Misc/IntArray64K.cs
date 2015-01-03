using BinarySerialization;

namespace BinarySerializer.Test.Misc
{
    public class IntArray64K
    {
        [FieldLength(65536 * sizeof(int))]
        public int[] Array { get; set; }
    }
}

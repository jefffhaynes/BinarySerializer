using BinarySerialization;

namespace BinarySerializer.Test.Count
{
    public class PrimitiveArrayBindingClass
    {
        [FieldOrder(0)]
        public int ItemCount { get; set; }

        [FieldOrder(1)]
        [FieldCount("ItemCount")]
        public int[] Ints { get; set; }
    }
}

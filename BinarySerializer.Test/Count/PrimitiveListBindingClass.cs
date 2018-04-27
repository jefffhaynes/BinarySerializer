using System.Collections.Generic;

namespace BinarySerialization.Test.Count
{
    public class PrimitiveListBindingClass
    {
        [FieldOrder(0)]
        public int ItemCount { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(ItemCount))]
        public List<int> Ints { get; set; }
    }
}
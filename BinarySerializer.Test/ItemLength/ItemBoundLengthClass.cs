using System.Collections.Generic;

namespace BinarySerialization.Test.ItemLength
{
    public class ItemBoundLengthClass
    {
        [FieldOrder(0)]
        public int ItemLength { get; set; }

        [FieldOrder(1)]
        [ItemLength("ItemLength")]
        public List<string> Items { get; set; }
    }
}
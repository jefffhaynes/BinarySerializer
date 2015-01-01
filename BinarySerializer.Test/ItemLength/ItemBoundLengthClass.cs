using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test.ItemLength
{
    public class ItemBoundLengthClass
    {
        [FieldOrder(0)]
        public int ItemLength { get; set; }

        [FieldOrder(1)]
        [ItemLength("ItemLength")]
        public List<string> List { get; set; } 
    }
}
using System.Collections.Generic;

namespace BinarySerialization.Test.ItemSubtype
{
    public class ItemSubtypeClass
    {
        public byte Indicator { get; set; }

        [ItemSubtype("Indicator", 1, typeof(ItemTypeA))]
        [ItemSubtype("Indicator", 2, typeof(ItemTypeB))]
        public List<IItemSubtype> Items { get; set; }
    }
}

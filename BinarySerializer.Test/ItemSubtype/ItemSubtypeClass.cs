using System.Collections.Generic;

namespace BinarySerialization.Test.ItemSubtype
{
    public class ItemSubtypeClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [ItemSubtype("Indicator", 1, typeof(ItemTypeA))]
        [ItemSubtype("Indicator", 2, typeof(ItemTypeB))]
        [ItemSubtype("Indicator", 3, typeof(CustomItem))]
        [ItemSubtypeDefault(typeof(ItemTypeA))]
        public List<IItemSubtype> Items { get; set; }
    }
}

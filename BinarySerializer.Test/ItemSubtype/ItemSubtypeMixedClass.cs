using System.Collections.Generic;

namespace BinarySerialization.Test.ItemSubtype
{
    public class ItemSubtypeMixedClass
    {
        [FieldOrder(0)]
        public byte Key { get; set; }

        [FieldOrder(1)]
        [ItemSubtype("Key", 2, typeof(ItemTypeB))]
        [ItemSubtypeFactory("Key", typeof(ItemSubtypeFactory))]
        public List<IItemSubtype> Value { get; set; }
    }
}

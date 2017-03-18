using System.Collections.Generic;

namespace BinarySerialization.Test.ItemSubtype
{
    public class ItemSubtypeFactoryWithDefaultClass
    {
        [FieldOrder(0)]
        public byte Key { get; set; }

        [FieldOrder(1)]
        [ItemSubtypeFactory("Key", typeof(ItemSubtypeFactory))]
        [ItemSubtypeDefault(typeof(DefaultItemType))]
        public List<IItemSubtype> Items { get; set; }
    }
}

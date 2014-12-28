using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test.UntilItem
{
    public class UntilItemContainer
    {
        public uint StuffBefore { get; set; }

        [ItemSerializeUntil("LastItem", "Yep")]
        public List<UntilItemClass> Items { get; set; }

        [ItemSerializeUntil("LastItem", "Yep", ExcludeLastItem = true)]
        public List<UntilItemClass> ItemsLastItemExcluded { get; set; }

        public uint StuffAfter { get; set; }
    }
}

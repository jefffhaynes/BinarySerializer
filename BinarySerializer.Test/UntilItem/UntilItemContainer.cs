using System.Collections.Generic;
using System.IO;
using BinarySerialization;

namespace BinarySerializer.Test.UntilItem
{
    public class UntilItemContainer
    {
        [FieldOrder(0)]
        public uint StuffBefore { get; set; }

        [FieldOrder(1)]
        [ItemSerializeUntil("LastItem", "Yep")]
        public List<UntilItemClass> Items { get; set; }

        [FieldOrder(2)]
        [ItemSerializeUntil("LastItem", "Yep", ExcludeLastItem = true)]
        public List<UntilItemClass> ItemsLastItemExcluded { get; set; }

        [FieldOrder(3)]
        public string SerializeUntilField { get; set; }

        [FieldOrder(4)]
        [ItemSerializeUntil("LastItem", Path = "SerializeUntilField")]
        public List<UntilItemClass> BoundItems { get; set; }

        [FieldOrder(5)]
        public uint StuffAfter { get; set; }
    }
}

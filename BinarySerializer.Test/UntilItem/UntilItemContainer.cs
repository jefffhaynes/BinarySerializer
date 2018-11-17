using System.Collections.Generic;

namespace BinarySerialization.Test.UntilItem
{
    public class UntilItemContainer
    {
        [FieldOrder(0)]
        public uint StuffBefore { get; set; }

        [FieldOrder(1)]
        [ItemSerializeUntil("LastItem", "Yep")]
        public List<UntilItemClass> Items { get; set; }

        [FieldOrder(2)]
        [ItemSerializeUntil("LastItem", "Yep", LastItemMode = LastItemMode.Discard)]
        public List<UntilItemClass> ItemsLastItemExcluded { get; set; }

        [FieldOrder(3)]
        public string SerializeUntilField { get; set; }

        [FieldOrder(4)]
        [ItemSerializeUntil("LastItem", Path = "SerializeUntilField")]
        public List<UntilItemClass> BoundItems { get; set; }

        [FieldOrder(5)]
        public uint StuffAfter { get; set; }

        [FieldOrder(6)]
        [ItemSerializeUntil("Type", (int)UntilItemEnum.End)]
        public List<UntilItemClass> EnumTerminationItems { get; set; }

        [FieldOrder(7)]
        public uint MoreStuffAfter { get; set; }
    }
}
using System.Collections.Generic;

namespace BinarySerialization.Test.UntilItem
{
    public class Section
    {
        [FieldOrder(0)]
        public UntilItemSimpleClass Header { get; set; }

        [FieldOrder(1)]
        [ItemSerializeUntil("Type", UntilItemEnum.Header, LastItemMode = LastItemMode.Defer)]
        public List<UntilItemSimpleClass> Items { get; set; } 
    }
}

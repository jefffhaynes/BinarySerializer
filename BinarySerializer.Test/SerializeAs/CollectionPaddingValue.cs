using System.Collections.Generic;

namespace BinarySerialization.Test.SerializeAs
{
    public class CollectionPaddingValue
    {
        [SerializeAs(PaddingValue = (byte) ' ')]
        [ItemLength(2)]
        public List<string> Items { get; set; }
    }
}

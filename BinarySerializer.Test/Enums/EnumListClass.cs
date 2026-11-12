using System.Collections.Generic;

namespace BinarySerialization.Test.Enums
{
    public class EnumListClass
    {
        [FieldOrder(0)]
        public byte Count { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<ColorValues> Colors { get; set; }
    }
}

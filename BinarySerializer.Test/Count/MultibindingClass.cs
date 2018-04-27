using System.Collections.Generic;

namespace BinarySerialization.Test.Count
{
    public class MultibindingClass
    {
        [FieldOrder(0)]
        public byte Count { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        [FieldCount(nameof(Count2), BindingMode = BindingMode.OneWayToSource)]
        public List<string> Items { get; set; }

        [FieldOrder(2)]
        public byte Count2 { get; set; }
    }
}
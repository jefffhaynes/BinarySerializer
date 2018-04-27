using System.Collections.Generic;

namespace BinarySerialization.Test.Count
{
    public class BoundCountClass
    {
        [FieldOrder(0)]
        public ushort FieldCountField { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(FieldCountField))]
        public List<string> Field { get; set; }
    }
}
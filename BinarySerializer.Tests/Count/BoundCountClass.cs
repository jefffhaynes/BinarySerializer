using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerialization.Test.Count
{
    public class BoundCountClass
    {
        [FieldOrder(0)]
        public ushort FieldCountField { get; set; }

        [FieldOrder(1)]
        [FieldCount("FieldCountField")]
        public List<string> Field { get; set; }
    }
}
using System;

namespace BinarySerialization.Test.Ignore
{
    public class IgnoreObjectClass
    {
        [FieldOrder(0)]
        public int FirstField { get; set; }

        [Ignore]
        public object IgnoreMe { get; set; }

        [Ignore]
        public DateTime DateTime { get; set; }

        [FieldOrder(1)]
        public int LastField { get; set; }
    }
}

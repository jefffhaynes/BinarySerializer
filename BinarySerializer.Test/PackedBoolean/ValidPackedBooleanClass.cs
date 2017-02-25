using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerialization.Test.PackedBoolean
{
    public class ValidPackedBooleanClass
    {
        [FieldOrder(0)] public long BooleanArrayCount { get; set; }
        [FieldOrder(1)] public long BooleanArrayLength { get; set; }

        [FieldCount(nameof(BooleanArrayCount)), FieldLength(nameof(BooleanArrayLength))]
        [FieldOrder(2)]
        public bool[] BooleanArray { get; set; }
    }
}

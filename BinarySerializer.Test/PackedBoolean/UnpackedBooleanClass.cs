using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerialization.Test.PackedBoolean
{
    public class UnpackedBooleanClass
    {
        [FieldOrder(0)] public long UnpackedArrayCount { get; set; }
        [FieldOrder(1)] public long UnpackedArrayLength { get; set; }

        [FieldCount(nameof(UnpackedArrayCount)), FieldLength(nameof(UnpackedArrayLength))]
        [FieldOrder(2)]
        public bool[] UnpackedArray { get; set; }
    }
}

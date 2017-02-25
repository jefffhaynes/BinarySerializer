using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerialization.Test.PackedBoolean
{
    public class ConstantSizePackedBooleanClass
    {
        [Ignore] public const int CountConstraint = 20;
        [Ignore] public const int LengthConstraint = 2;

        [FieldCount(CountConstraint)]
        [FieldOrder(0)]
        public bool[] ConstantCountArray { get; set; }

        [FieldLength(2)]
        [FieldOrder(1)]
        public bool[] ConstantLengthArray { get; set; }
    }
}

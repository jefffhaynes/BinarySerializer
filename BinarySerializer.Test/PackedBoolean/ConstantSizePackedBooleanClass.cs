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
        [FieldOrder(0), Pack]
        public bool[] ConstantCountArray { get; set; }

        [FieldLength(LengthConstraint)]
        [FieldOrder(1), Pack]
        public bool[] ConstantLengthArray { get; set; }
    }
}

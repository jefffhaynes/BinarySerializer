using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerialization.Test.BitLength
{
    public class BitLengthOverflowLsbClass
    {
        [FieldOrder(0)]
        [FieldBitLength(2)]
        public int Value;

        [FieldOrder(1)]
        [FieldBitLength(4)]
        public int Value2;

        [FieldOrder(2)]
        [FieldBitLength(2)]
        public int Value3;
    }

    public class BitLengthOverflowMsbClass
    {
        [FieldOrder(0)]
        [FieldBitLength(2)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public int Value;

        [FieldOrder(1)]
        [FieldBitLength(4)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public int Value2;

        [FieldOrder(2)]
        [FieldBitLength(2)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public int Value3;
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue180
{
    [TestClass]
    public class Issue180Tests : TestBase
    {
        public class BitFieldsPrecededByByte
        {
            [FieldOrder(0)]
            public byte First;

            [FieldOrder(1)]
            [FieldBitLength(7)]
            public byte SevenBits;

            [FieldOrder(2)]
            [FieldBitLength(1)]
            public byte OneBit;
        }

        [TestMethod]
        public void TestOutOfMemory()
        {
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine($"Trying to deserialize array of partial {nameof(BitFieldsPrecededByByte)} with array size {i}");
                var serialized = new byte[i];
                // This call will never return and consumes all available memory
                var _ = Deserialize<BitFieldsPrecededByByte[]>(serialized);
            }
        }
    }
}

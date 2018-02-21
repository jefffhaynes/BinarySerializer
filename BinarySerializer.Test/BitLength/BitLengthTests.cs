using System;
using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.BitLength
{
    public class BitLengthTests : TestBase
    {
        [Fact]
        public void LengthTest()
        {
            var expected = new BitLengthClass
            {
                A = 0b1_0110_1110_1111_0111_1101,
                B = 0b111,
                C = (TypeCode) 0b1101,
                Internal = new InternalBitLengthClass {Value = 0b1111},
                Internal2 = new InternalBitLengthClass {Value = 0b10101010}
            };

            var actual = Roundtrip(expected, new byte[] {0xb7, 0x7b, 0xef, 0xdf, 0xa0});
            Assert.Equal(expected.A, actual.A);
            Assert.Equal(expected.B, actual.B);
            Assert.Equal(expected.C, actual.C);
            Assert.Equal(expected.Internal.Value, actual.Internal.Value);
            Assert.Equal(0b1010, actual.Internal2.Value);
        }

        [Fact]
        public void TestBitLengthValue()
        {
            var expected = new BitLengthValueClass
            {
                Value = new InternalBitLengthValueClass
                {
                    Value = 1,
                    Value2 = 1
                },
                Value2 = 0x11
            };

            var acutal = Roundtrip(expected);

            Assert.Equal(expected.Value.Value, acutal.Value.Value);
            Assert.Equal(expected.Value.Value2, acutal.Value.Value2);
            Assert.Equal(acutal.Crc2, acutal.Crc);
        }

        [Fact]
        public void BitLengthBoolTest()
        {
            var expected = new List<BitLengthBoolClass>
            {
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false},
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false},
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false},
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false}
            };

            var actual = Roundtrip(expected, new byte[]{0xaa});

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Value, actual[i].Value);
            }
        }
    }
}


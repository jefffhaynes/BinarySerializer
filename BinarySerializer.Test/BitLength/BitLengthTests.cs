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
                C = 0b1101,
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
    }
}

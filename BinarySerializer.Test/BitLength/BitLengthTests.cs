using Xunit;

namespace BinarySerialization.Test.BitLength
{
    public class BitLengthTests : TestBase
    {
        [Fact]
        public void LengthTest()
        {
            var expected = new BitLengthClass {A = 0x02, B = 0x2a};
            var actual = Roundtrip(expected, new byte[] {0xaa, 0x00});
            Assert.Equal(expected.A, actual.A);
            Assert.Equal(expected.B, actual.B);
        }
    }
}

using System.IO;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue122
{
    public class Issue122Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var foo = new Foo();
            Roundtrip(foo, new byte[] {0x21});
        }

        [Fact]
        public void LaserFrameTest()
        {
            var expected = new LaserFrame {X = 0x123, Y = 0x456, R = 0x78, G = 0x90, B = 0xAB};
            var actual = Roundtrip(expected, new byte[] {0x23, 0x61, 0x45, 0x78, 0x90, 0xAB});

            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
            Assert.Equal(expected.Y, actual.Y);
            Assert.Equal(expected.Y, actual.Y);
            Assert.Equal(expected.B, actual.B);
        }
    }
}

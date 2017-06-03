using Xunit;

namespace BinarySerialization.Test.Alignment
{
    
    public class AlignmentTests : TestBase
    {
        [Fact]
        public void AlignmentTest()
        {
            var actual = RoundtripReverse<AlignmentClass>(new byte[]
            {
                0x2, 0x0, 0x0, 0x0,
                (byte) 'h', (byte) 'i', 0, 0
            });

            Assert.Equal(2, actual.Length);
            Assert.Equal("hi", actual.Value);
        }

        [Fact]
        public void BoundAlignmentTest()
        {
            var actual = RoundtripReverse<BoundAlignmentClass>(new byte[]
            {
                0x2, 0x4, 0x0, 0x0,
                (byte) 'h', (byte) 'i', 0, 0
            });

            Assert.Equal(2, actual.Length);
            Assert.Equal(4, actual.Alignment);
            Assert.Equal("hi", actual.Value);
        }

        [Fact]
        public void LeftAlignmentTest()
        {
            var actual = RoundtripReverse<LeftAlignmentClass>(new byte[]
            {
                0x1, 0x0, 0x0, 0x0,
                0x2,
                0x3
            });

            Assert.Equal((byte)1, actual.Header);
            Assert.Equal((byte)2, actual.Value);
            Assert.Equal((byte)3, actual.Trailer);
        }

        [Fact]
        public void RightAlignmentTest()
        {
            var actual = RoundtripReverse<RightAlignmentClass>(new byte[]
            {
                0x1,
                0x2, 0x0, 0x0, 
                0x3
            });

            Assert.Equal((byte)1, actual.Header);
            Assert.Equal((byte)2, actual.Value);
            Assert.Equal((byte)3, actual.Trailer);
        }

        [Fact]
        public void MixedAlignmentTest()
        {
            var actual = RoundtripReverse<MixedAlignmentClass>(new byte[]
            {
                0x1, 0x0, 0x0, 0x0,
                0x2, 0x0,
                0x3
            });

            Assert.Equal((byte)1, actual.Header);
            Assert.Equal((byte)2, actual.Value);
            Assert.Equal((byte)3, actual.Trailer);
        }
    }
}

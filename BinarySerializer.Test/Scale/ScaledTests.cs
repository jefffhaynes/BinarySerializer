using Xunit;

namespace BinarySerialization.Test.Scale
{
    public class ScaledTests : TestBase
    {
        [Fact]
        public void ScaleTest()
        {
            var expected = new ScaledValueClass {Value = 3};
            var actual = Roundtrip(expected, new byte[] {0x6});
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void NegativeScaleTest()
        {
            var expected = new ScaledValueClass { Value = -3 };
            var actual = Roundtrip(expected, new byte[] { 250 });
            Assert.Equal(expected.Value, actual.Value);
        }
    }
}

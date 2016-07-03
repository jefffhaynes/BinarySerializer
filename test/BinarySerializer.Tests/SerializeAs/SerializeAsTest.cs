using Xunit;

namespace BinarySerialization.Test.SerializeAs
{
        public class SerializeAsTest : TestBase
    {
        [Fact]
        public void SerializeIntAsSizedStringTest()
        {
            var expected = new SizedStringClass<int> {Value = 33};
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes(expected.Value.ToString()));

            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void SerializeAsLengthPrefixedStringTest()
        {
            var expected = new LengthPrefixedStringClass {Value = new string('c', ushort.MaxValue)};
            var actual = Roundtrip(expected, ushort.MaxValue + 3);

            Assert.Equal(expected.Value, actual.Value);
        }
    }
}

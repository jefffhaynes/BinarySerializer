using Xunit;

namespace BinarySerialization.Test.Issues.Issue82
{
    public class Issue82Tests : TestBase 
    {
        [Fact]
        public void TestVersionConverter()
        {
            var expected = new SerializeWhenClass
            {
                Version = 33,
                Value = true
            };

            var actual = Roundtrip(expected);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void TestVersionConverterMiss()
        {
            var expected = new SerializeWhenClass
            {
                Version = 25,
                Value = true
            };

            var actual = Roundtrip(expected);
            Assert.False(actual.Value);
        }
    }
}

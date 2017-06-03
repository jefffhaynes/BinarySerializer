using Xunit;

namespace BinarySerialization.Test.Ignore
{
    
    public class IgnoreTests : TestBase
    {
        [Fact]
        public void IgnoreObjectTest()
        {
            var expected = new IgnoreObjectClass {FirstField = 1, IgnoreMe = "hello", LastField = 2};
            var actual = Roundtrip(expected, 8);

            Assert.Equal(expected.FirstField, actual.FirstField);
            Assert.Null(actual.IgnoreMe);
            Assert.Equal(expected.LastField, actual.LastField);
        }
    }
}
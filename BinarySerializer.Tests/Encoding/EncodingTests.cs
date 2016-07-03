using Xunit;

namespace BinarySerialization.Test.Encoding
{
        public class EncodingTests : TestBase
    {
        [Fact]
        public void TestEncoding()
        {
            var expected = new EncodingClass {Name = "غدير"};
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name);
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Name, actual.Name);
        }
    }
}

using Xunit;

namespace BinarySerialization.Test.Issues.Issue80
{
    public class Issue80Tests : TestBase
    {
        [Fact]
        public void Test0Xff()
        {
            var expected = new CustomField();
            var actual = Roundtrip(expected, new byte[] {0xff});
            Assert.Null(actual.Value);
        }

        [Fact]
        public void TestNormal()
        {
            var expected = new CustomField {Value = "hi"};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Value, actual.Value);
        }
    }
}

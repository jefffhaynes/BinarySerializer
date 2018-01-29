using Xunit;

namespace BinarySerialization.Test.Issues.Issue98
{
    public class Issue89Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new Order {I = 5, RId = 10};
            var actual = Roundtrip(expected);
        }
    }
}

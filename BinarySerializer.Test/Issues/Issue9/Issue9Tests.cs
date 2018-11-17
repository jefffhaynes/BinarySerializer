using Xunit;

namespace BinarySerialization.Test.Issues.Issue9
{
    
    public class Issue9Tests : TestBase
    {
        [Fact]
        public void TestMethod()
        {
            var expected = new ElementClass();
            var actual = Roundtrip(expected);
        }
    }
}
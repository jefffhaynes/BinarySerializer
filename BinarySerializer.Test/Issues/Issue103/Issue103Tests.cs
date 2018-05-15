using Xunit;

namespace BinarySerialization.Test.Issues.Issue103
{
    public class Issue103Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new MultipleFieldsCrc32 {Msgs = "a"};
            var actual = Roundtrip(expected);
        }
    }
}

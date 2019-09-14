using Xunit;

namespace BinarySerialization.Test.Issues.Issue139
{
    public class Issue139Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new Question();
            var actual = Roundtrip(expected);
        }
    }
}

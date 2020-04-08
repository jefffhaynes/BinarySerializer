using Xunit;

namespace BinarySerialization.Test.Issues.Issue140
{
    public class Issue140Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new Question
            {
                Domain = new Domain()
            };

            Roundtrip(expected);
        }
    }
}

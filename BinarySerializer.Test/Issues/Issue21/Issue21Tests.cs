using Xunit;

namespace BinarySerialization.Test.Issues.Issue21
{
    
    public class Issue21Tests : TestBase
    {
        [Fact]
        public void TestIssue21()
        {
            Roundtrip(new FailingClass());
        }
    }
}
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue149
{
    public class Issue149Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var metrics = new FileMetrics();
            var context = new TestContext();
            Roundtrip(metrics, context);
        }
    }
}

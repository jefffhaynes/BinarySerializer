using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue21
{
    [TestClass]
    public class Issue21Tests : TestBase
    {
        [TestMethod]
        public void TestIssue21()
        {
            Roundtrip(new FailingClass());
        }
    }
}
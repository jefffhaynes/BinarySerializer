using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue98
{
    [TestClass]
    public class Issue89Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var expected = new Order {I = 5, RId = 10};
            var actual = Roundtrip(expected);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue103
{
    [TestClass]
    public class Issue103Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var expected = new MultipleFieldsCrc32 {Msgs = "a"};
            var actual = Roundtrip(expected);
        }
    }
}

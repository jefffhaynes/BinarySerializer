using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue34
{
    [TestClass]
    public class Issue34Tests : TestBase
    {
        [TestMethod]
        public void RoundtripString()
        {
            var expected = new S7String {Value = new InternalS7String("hello")};
            var actual = Roundtrip(expected, 7);
        }
    }
}
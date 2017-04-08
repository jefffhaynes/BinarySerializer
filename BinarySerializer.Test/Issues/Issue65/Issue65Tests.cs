using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue65
{
    [TestClass]
    public class Issue65Tests : TestBase
    {
        [TestMethod]
        public void CountTest()
        {
            Roundtrip(new TestClass(), 10);
        }

        [TestMethod]
        public void ComplexLengthTest()
        {
            Roundtrip(new ComplexTestClass {ComplexClass = new ComplexClass()}, 5);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test_BinarySerialzer;

namespace BinarySerialization.Test.Issues.Issue170
{
    [TestClass]
    public class Issue170Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var expected = new TestClass();
            Roundtrip(expected);
        }
    }
}
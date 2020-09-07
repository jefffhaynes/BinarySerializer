using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue88
{
    [TestClass]
    public class Issue88Tests : TestBase
    {
        [TestMethod]
        public void FieldValueTest()
        {
            var expected = new ParentClass {Value = 5};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue82
{
    [TestClass]
    public class Issue82Tests : TestBase 
    {
        [TestMethod]
        public void TestVersionConverter()
        {
            var expected = new SerializeWhenClass
            {
                Version = 33,
                Value = true
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void TestVersionConverterMiss()
        {
            var expected = new SerializeWhenClass
            {
                Version = 25,
                Value = true
            };

            var actual = Roundtrip(expected);
            Assert.IsFalse(actual.Value);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Ignore
{
    [TestClass]
    public class IgnoreTests : TestBase
    {
        [TestMethod]
        public void IgnoreObjectTest()
        {
            var expected = new IgnoreObjectClass {FirstField = 1, IgnoreMe = "hello", LastField = 2};
            var actual = Roundtrip(expected, 8);

            Assert.AreEqual(expected.FirstField, actual.FirstField);
            Assert.IsNull(actual.IgnoreMe);
            Assert.AreEqual(expected.LastField, actual.LastField);
        }

        [TestMethod]
        public void IgnoreDerivedObjectTest()
        {
            var expected = new IgnoreDerivedObjectClass { FirstField = 1, IgnoreMe = "hello", LastField = 2 };
            var actual = Roundtrip(expected, 12);

            Assert.AreEqual(expected.FirstField, actual.FirstField);
            Assert.IsNull(actual.IgnoreMe);
            Assert.AreEqual(expected.LastField, actual.LastField);
        }

        [TestMethod]
        public void IgnoreSubclassClass()
        {
            var expected = new IgnoreSubclassContainer {Base = new IgnoreSubclassClass()};
            Roundtrip(expected, 12);
        }
    }
}

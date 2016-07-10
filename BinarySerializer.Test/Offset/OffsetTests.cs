using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Offset
{
    [TestClass]
    public class OffsetTests : TestBase
    {
        [TestMethod]
        public void ConstOffsetTest()
        {
            var expected = new ConstOffsetClass {Field = "FieldValue"};
            var actual = Roundtrip(expected, 100 + expected.Field.Length + 1);
            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void BoundOffsetTest()
        {
            var expected = new BoundOffsetClass {FieldOffsetField = 1000, Field = "FieldValue"};
            var actual = Roundtrip(expected, expected.FieldOffsetField + expected.Field.Length + 1);
            Assert.AreEqual(expected.Field, actual.Field);
        }
    }
}
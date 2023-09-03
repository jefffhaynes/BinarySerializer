using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Offset
{
    [TestClass]
    public class OffsetTests : TestBase
    {
        [TestMethod]
        public void ConstOffsetTest()
        {
            var stringVal = "FieldValue"; // 10 length
            var expected = new ConstOffsetClass {FieldStringLength = (uint)stringVal.Length, FieldString = stringVal, FieldUint = 100};
            var actual = Roundtrip(expected, 100 + expected.FieldStringLength);
            Assert.AreEqual(expected.FieldStringLength, actual.FieldStringLength);
            Assert.AreEqual(expected.FieldString, actual.FieldString);
            Assert.AreEqual(expected.FieldUint, actual.FieldUint);
        }

        [TestMethod]
        public void BoundOffsetTest()
        {
            var expected = new BoundOffsetClass {FieldOffsetField = 1000, FieldString = "FieldValue", FieldUint = 100};
            var actual = Roundtrip(expected, expected.FieldOffsetField + expected.FieldString.Length + 1);
            Assert.AreEqual(expected.FieldOffsetField, actual.FieldOffsetField);
            Assert.AreEqual(expected.FieldString, actual.FieldString);
            Assert.AreEqual(expected.FieldUint, actual.FieldUint);
        }

        [TestMethod]
        public void BoundOffsetCurrentNoRewindTest()
        {
            var expected = new BoundOffsetCurrentNoRewindClass { FieldOffsetField = 50, Field = 404, LastUInt = 9};
            var actual = Roundtrip(expected, sizeof(uint) + expected.FieldOffsetField + sizeof(uint) + sizeof(uint));
            Assert.AreEqual(expected.Field, actual.Field);
            Assert.AreEqual(expected.LastUInt, actual.LastUInt);
        }

        [TestMethod]
        public void BoundOffsetJumpyNoRewindTest()
        {
            var expected = new BoundOffsetJumpyNoRewindClass { FieldOffsetField1 = 100, Field1 = 104, FieldOffsetField2 = 200, Field2 = 204, FieldOffsetField3 = sizeof(uint), Field3 = 304 };
            var actual = Roundtrip(expected, expected.FieldOffsetField2 + sizeof(uint) + sizeof(uint) + expected.FieldOffsetField3 + sizeof(uint));
            Assert.AreEqual(expected.Field1, actual.Field1);
            Assert.AreEqual(expected.Field2, actual.Field2);
            Assert.AreEqual(expected.Field3, actual.Field3);
        }
    }
}
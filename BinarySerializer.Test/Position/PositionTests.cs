using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Position
{
    [TestClass]
    public class PositionTests : TestBase
    {
        [TestMethod]
        public void BoundPositionBeginTest()
        {
            var expected = new BoundPositionBeginClass { FieldOffsetField = 500, Field = 404, LastUInt = 5, RepeatFieldOffsetField = 500};
            var actual = Roundtrip(expected, expected.FieldOffsetField + sizeof(uint) + sizeof(uint));
            Assert.AreEqual(expected.Field, actual.Field);
            Assert.AreEqual(expected.LastUInt, actual.LastUInt);
            Assert.AreEqual(expected.FieldOffsetField, actual.RepeatFieldOffsetField);
        }

        [TestMethod]
        public void BoundPositionBeginRewindTest()
        {
            var expected = new BoundPositionBeginRewindClass { FieldOffsetField = 500, Field = 404, SecondUint = 5 };
            var actual = Roundtrip(expected, expected.FieldOffsetField + sizeof(uint));
            Assert.AreEqual(expected.Field, actual.Field);
            Assert.AreEqual(expected.SecondUint, actual.SecondUint);
        }

        [TestMethod]
        public void BoundPositionCurrentTest()
        {
            var expected = new BoundPositionCurrentClass { FieldOffsetField = 1, Field = 404, LastInt = 5 };
            var actual = Roundtrip(expected, sizeof(uint) + expected.FieldOffsetField + sizeof(uint) + sizeof(uint));
            Assert.AreEqual(expected.Field, actual.Field);
            Assert.AreEqual(expected.LastInt, actual.LastInt);
        }

        [TestMethod]
        public void BoundPositionJumpyTest()
        {
            var expected = new BoundPositionJumpyClass { FieldOffsetField1 = 100, Field1 = 104, FieldOffsetField2 = 200, Field2 = 204, FieldOffsetField3 = sizeof(uint), Field3 = 304 };
            var actual = Roundtrip(expected, expected.FieldOffsetField2 + sizeof(uint) + sizeof(uint) + expected.FieldOffsetField3 + sizeof(uint));
            Assert.AreEqual(expected.Field1, actual.Field1);
            Assert.AreEqual(expected.Field2, actual.Field2);
            Assert.AreEqual(expected.Field3, actual.Field3);
        }
    }
}
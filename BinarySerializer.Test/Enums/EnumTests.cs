using System;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Enums
{
    [TestClass]
    public class EnumTests : TestBase
    {
        [TestMethod]
        public void GetEnumSerializedValuesBaseValuesTest()
        {
            SerializedType serializedType;
            var enumValues = typeof(BaseTypeEnumValues).GetEnumSerializedValues(out serializedType);

            Assert.AreEqual(BaseTypeEnumValues.A, enumValues[BaseTypeEnumValues.A]);
            Assert.AreEqual(BaseTypeEnumValues.B, enumValues[BaseTypeEnumValues.B]);
            Assert.AreEqual(BaseTypeEnumValues.C, enumValues[BaseTypeEnumValues.C]);
            Assert.AreEqual(3, enumValues.Count);
        }

        [TestMethod]
        public void GetEnumSerializedValuesNamedValuesTest()
        {
            SerializedType serializedType;
            var enumValues = typeof(NamedEnumValues).GetEnumSerializedValues(out serializedType);

            Assert.AreEqual("Alpha", enumValues[NamedEnumValues.A]);
            Assert.AreEqual("Bravo", enumValues[NamedEnumValues.B]);
            Assert.AreEqual("C", enumValues[NamedEnumValues.C]);
            Assert.AreEqual(3, enumValues.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumSerializedValuesPartialNamedValuesShouldThrowInvalidOperationTest()
        {
            SerializedType serializedType;
            typeof(PartiallyNamedEnumValues).GetEnumSerializedValues(out serializedType);
        }

        [TestMethod]
        public void BaseTypeEnumValuesTest()
        {
            var expected = new BaseTypeEnumClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected, 2);
            Assert.AreEqual(BaseTypeEnumValues.B, actual.Field);
        }

        [TestMethod]
        public void NamedEnumValuesTest()
        {
            var expected = new NamedEnumClass { Field = NamedEnumValues.B };
            var actual = Roundtrip(expected, 6);
            Assert.AreEqual(NamedEnumValues.B, actual.Field);
        }

        [TestMethod]
        public void IncompleteEnumValuesTest()
        {
            var expected = new IncompleteEnumClass { Field = (IncompleteEnumValues)2 };
            var actual = Roundtrip(expected, 4);
            Assert.AreEqual((IncompleteEnumValues)2, actual.Field);
        }
    }
}

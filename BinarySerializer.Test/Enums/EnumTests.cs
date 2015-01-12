using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Enums
{
    [TestClass]
    public class EnumTests : TestBase
    {
        [TestMethod]
        public void BasicEnumTest()
        {
            var expected = new BaseTypeEnumClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected, sizeof(BaseTypeEnumValues));

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void EnumAsStringTest()
        {
            var expected = new BaseTypeEnumAsStringClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected, new byte[] {(byte) 'B', 0x0});

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void NamedEnumTest()
        {
            var expected = new NamedEnumClass { Field = NamedEnumValues.B };
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes("Bravo"));

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void NamedEnumTest2()
        {
            var expected = new NamedEnumClass { Field = NamedEnumValues.C };
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes("C"));

            Assert.AreEqual(expected.Field, actual.Field);
        }
    }
}

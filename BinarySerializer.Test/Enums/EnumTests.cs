using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Enums
{
    [TestClass]
    public class EnumTests : TestBase
    {
        [TestMethod]
        public void BasicEnumTest()
        {
            var expected = new BaseTypeEnumClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected, sizeof (BaseTypeEnumValues));

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void BasicSignedEnumTest()
        {
            var expected = new BaseTypeSignedEnumClass {Field = BaseTypeSignedEnumValues.NegativeValue};
            var actual = Roundtrip(expected, sizeof (BaseTypeSignedEnumValues));

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
        public void EnumAsStringTest2()
        {
            var expected = new BaseTypeEnumAsStringClass2 { Field = BaseTypeEnumValues.B };
            var actual = Roundtrip(expected, new byte[] { (byte)'B', 0x1 });

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void NamedEnumTest()
        {
            var expected = new NamedEnumClass {Field = NamedEnumValues.B};
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes("Bravo" + char.MinValue));

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void NamedEnumTest2()
        {
            var expected = new NamedEnumClass {Field = NamedEnumValues.C};
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes("C\0"));

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void NullableEnumTest()
        {
            var expected = new NullableEnumClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void NegativeEnumTest()
        {
            var expected = new NegativeEnumClass {Value = NegativeEnum.A};
            var actual = Roundtrip(expected, new[] {(byte) 0xff, (byte) 0xff});

            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void EnumArrayBoundCountTest()
        {
            var expected = new EnumArrayClass
            {
                Count = 2,
                Colors = new[] {ColorValues.Red, ColorValues.Blue}
            };
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Count, actual.Count);
            CollectionAssert.AreEqual(expected.Colors, actual.Colors);
        }

        [TestMethod]
        public void EnumArrayRawDeserializeTest()
        {
            /* Red = 1, Blue = 3 as little-endian int32 (int-backed enum elements). */
            var data = new byte[] {2, 1, 0, 0, 0, 3, 0, 0, 0};
            var actual = Deserialize<EnumArrayClass>(new MemoryStream(data));

            Assert.AreEqual((byte) 2, actual.Count);
            CollectionAssert.AreEqual(new[] {ColorValues.Red, ColorValues.Blue}, actual.Colors);
        }

        [TestMethod]
        public void ByteEnumArrayConstCountTest()
        {
            var expected = new ByteEnumArrayClass
            {
                Field = new[] {BaseTypeEnumValues.A, BaseTypeEnumValues.C, BaseTypeEnumValues.B}
            };
            var actual = Roundtrip(expected, 3);

            CollectionAssert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void EnumListTest()
        {
            var expected = new EnumListClass
            {
                Count = 2,
                Colors = new System.Collections.Generic.List<ColorValues>
                    {ColorValues.Green, ColorValues.Red}
            };
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Count, actual.Count);
            CollectionAssert.AreEqual(expected.Colors, actual.Colors);
        }
    }
}
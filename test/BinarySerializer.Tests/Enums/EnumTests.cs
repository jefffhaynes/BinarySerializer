using Xunit;

namespace BinarySerialization.Test.Enums
{
        public class EnumTests : TestBase
    {
        [Fact]
        public void BasicEnumTest()
        {
            var expected = new BaseTypeEnumClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected, sizeof(BaseTypeEnumValues));

            Assert.Equal(expected.Field, actual.Field);
        }

        [Fact]
        public void EnumAsStringTest()
        {
            var expected = new BaseTypeEnumAsStringClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected, new byte[] {(byte) 'B', 0x0});

            Assert.Equal(expected.Field, actual.Field);
        }

        [Fact]
        public void NamedEnumTest()
        {
            var expected = new NamedEnumClass { Field = NamedEnumValues.B };
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes("Bravo"));

            Assert.Equal(expected.Field, actual.Field);
        }

        [Fact]
        public void NamedEnumTest2()
        {
            var expected = new NamedEnumClass { Field = NamedEnumValues.C };
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes("C"));

            Assert.Equal(expected.Field, actual.Field);
        }

        [Fact]
        public void NullableEnumTest()
        {
            var expected = new NullableEnumClass {Field = BaseTypeEnumValues.B};
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Field, actual.Field);
        }
    }
}

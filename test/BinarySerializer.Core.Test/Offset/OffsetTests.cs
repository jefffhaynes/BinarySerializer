using Xunit;

namespace BinarySerialization.Test.Offset
{
        public class OffsetTests : TestBase
    {
        [Fact]
        public void ConstOffsetTest()
        {
            var expected = new ConstOffsetClass {Field = "FieldValue"};
            var actual = Roundtrip(expected, 100 + expected.Field.Length + 1);
            Assert.Equal(expected.Field, actual.Field);
        }

        [Fact]
        public void BoundOffsetTest()
        {
            var expected = new BoundOffsetClass { FieldOffsetField = 1000, Field = "FieldValue" };
            var actual = Roundtrip(expected, expected.FieldOffsetField + expected.Field.Length + 1);
            Assert.Equal(expected.Field, actual.Field);
        }
    }
}

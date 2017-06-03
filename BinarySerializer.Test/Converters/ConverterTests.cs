using Xunit;

namespace BinarySerialization.Test.Converters
{
    
    public class ConverterTests : TestBase
    {
        [Fact]
        public void ConverterTest()
        {
            var expected = new ConverterClass {Field = "FieldValue"};
            var actual = Roundtrip(expected);
            Assert.Equal((double) expected.Field.Length/2, actual.HalfFieldLength);
            Assert.Equal(expected.Field, actual.Field);
        }
    }
}
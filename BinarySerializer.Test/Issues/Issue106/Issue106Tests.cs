using Xunit;

namespace BinarySerialization.Test.Issues.Issue106
{
    public class Issue106Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new Buffer
            {
                Field1 = 0x01,
                Field2 = 0x10,
                Field3 = 0x40,
                Field4 = 0xff
            };

            var actual = Roundtrip(expected, new byte[] {0x41, 0x80, 0xFF});
            Assert.Equal(expected.Field1, actual.Field1);
            Assert.Equal(expected.Field2, actual.Field2);
            Assert.Equal(expected.Field3, actual.Field3);
            Assert.Equal(expected.Field4, actual.Field4);
        }
    }
}

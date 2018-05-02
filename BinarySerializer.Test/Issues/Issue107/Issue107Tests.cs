using Xunit;

namespace BinarySerialization.Test.Issues.Issue107
{
    public class Issue107Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new ClassToSerialize {Field1 = 1, Field2 = 2, Field3 = 3, Field4 = 4, Field5 = 5};
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Field1, actual.Field1);
            Assert.Equal(expected.Field2, actual.Field2);
            Assert.Equal(expected.Field3, actual.Field3);
            Assert.Equal(expected.Field4, actual.Field4);
            Assert.Equal(expected.Field5, actual.Field5);
        }
    }
}

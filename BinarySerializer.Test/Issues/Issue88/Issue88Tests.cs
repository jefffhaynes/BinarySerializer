using Xunit;

namespace BinarySerialization.Test.Issues.Issue88
{
    public class Issue88Tests : TestBase
    {
        [Fact]
        public void FieldValueTest()
        {
            var expected = new ParentClass {Value = 5};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Value, actual.Value);
        }
    }
}

using BinarySerialization.Test;
using Xunit;

namespace BinarySerializer.Test.Issues.Issue57
{
    
    public class Issue57Tests : TestBase
    {
        [Fact]
        public void Roundtrip()
        {
            var expected = new BigEndianFloatClass
            {
                Value = -48.651363f
            };

            var actual = Roundtrip(expected, sizeof(float));
            Assert.Equal(expected.Value, actual.Value);
        }
    }
}

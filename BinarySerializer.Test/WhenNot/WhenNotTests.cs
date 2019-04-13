using Xunit;

namespace BinarySerialization.Test.WhenNot
{
    
    public class WhenNotTests : TestBase
    {
        [Fact]
        public void SimpleTest()
        {
            var expected = new WhenNotClass
            {
                ExcludeValue = true,
                Value = 100
            };

            var data = Serialize(expected);
            Assert.Single(data);

            expected.ExcludeValue = false;
            data = Serialize(expected);
            Assert.Equal(5, data.Length);
        }
    }
}

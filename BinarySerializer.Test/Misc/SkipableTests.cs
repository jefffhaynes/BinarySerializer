using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class SkipableTests : TestBase
    {
        [Fact]
        public void SkipTest()
        {
            var actual = Deserialize<SkipableContainerClass>(new byte[0]);
            Assert.Null(actual.Skipable);
        }
    }
}

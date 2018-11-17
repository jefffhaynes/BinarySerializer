using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class InvalidButIgnoredTests : TestBase
    {
        [Fact]
        public void InvalidButIgnoredTest()
        {
            Roundtrip(new InvalidButIgnoredContainerClass
            {
                InvalidButIgnored = new InvalidButIgnoredTypeClass()
            });
        }
    }
}

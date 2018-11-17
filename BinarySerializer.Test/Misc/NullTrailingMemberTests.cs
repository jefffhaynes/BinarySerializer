using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class NullTrailingMemberTests : TestBase
    {
        [Fact]
        public void NullTrailingMemberTest()
        {
            var container = new NullTrailingMemberClassContainer();

            Roundtrip(container, 12);

            container.Inner.OptionalParameter = 5;

            Roundtrip(container, 13);
        }
    }
}
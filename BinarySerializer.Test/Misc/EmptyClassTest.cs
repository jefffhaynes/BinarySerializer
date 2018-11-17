using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class EmptyClassTest : TestBase
    {
        [Fact]
        public void RoundtripEmptyClassTest()
        {
            Roundtrip(new EmptyClass());
        }
    }
}
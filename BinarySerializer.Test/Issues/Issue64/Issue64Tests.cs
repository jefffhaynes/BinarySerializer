using Xunit;

namespace BinarySerialization.Test.Issues.Issue64
{
    
    public class Issue64Tests : TestBase
    {
        //[Fact]
        public void AlignmentTest()
        {
            byte[] data = {
                0x05, 0x05, 0x05, 0x05,
                0x08, 0x70, 0x70, 0x70,
                0x64, 0x64, 0x64, 0x64,
                0x65, 0x65, 0x63, 0x63,
                0x63, 0x63, 0x63, 0x63,
                0x63, 0x63, 0x63, 0x63
            };

            var parent = RoundtripReverse<Parent>(data);
        }
    }
}

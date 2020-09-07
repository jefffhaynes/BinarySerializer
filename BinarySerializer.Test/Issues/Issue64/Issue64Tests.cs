using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue64
{
    [TestClass]
    public class Issue64Tests : TestBase
    {
        //[TestMethod]
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue205
{
    [TestClass]
    public class Issue205Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var expected = new Segment
            {
                Type = 0x00,
                PortSegment = new PortSegment
                {
                    ExtendedLinkAddress = false,
                    Port = 1,
                    LinkAddress = 32,
                },
            };

            var actual = Roundtrip(expected, new byte[] {0x01, 0x20});
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.PortSegment.ExtendedLinkAddress, actual.PortSegment.ExtendedLinkAddress);
            Assert.AreEqual(expected.PortSegment.Port, actual.PortSegment.Port);
            Assert.AreEqual(expected.PortSegment.LinkAddress, actual.PortSegment.LinkAddress);
        }
    }
}

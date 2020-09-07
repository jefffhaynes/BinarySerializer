using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue76
{
    [TestClass]
    public class Issue76Tests : TestBase
    {
        [TestMethod]
        public void TestExample()
        {
            var expected = new byte[]
            {
                0x00, 0x00, 0x00, 0x17,
                0x73, 0x4D, 0x4E, 0x20,
                0x53, 0x65, 0x74, 0x41, 0x63, 0x63, 0x65, 0x73, 0x73, 0x4D, 0x6F, 0x64, 0x65, 0x20,
                0x03, 0xF4, 0x72, 0x47, 0x44,
                0xB3
            };

            var packet = new Packet
            {
                Content = new PacketContent
                {
                    Payload = new SmnCommandContainer
                    {
                        Command = new SetAccessModeCommand()
                    }
                }
            };

            RoundtripBigEndian(packet, expected);
        }
    }
}

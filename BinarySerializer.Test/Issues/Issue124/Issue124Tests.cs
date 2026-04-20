using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue124
{
    [TestClass]
    public class Issue124Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var expected = new ApplicationMessage
            {
                Packets = new List<Packet>
                {
                    new Packet
                    {
                        PacketBody = new Packet1()
                    },
                    new Packet
                    {
                        PacketBody = new Packet1()
                    }
                }
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Packets.Count, actual.Packets.Count);
            Assert.IsInstanceOfType(actual.Packets[0].PacketBody, typeof(Packet1));
            Assert.IsInstanceOfType(actual.Packets[1].PacketBody, typeof(Packet1));
        }
    }
}

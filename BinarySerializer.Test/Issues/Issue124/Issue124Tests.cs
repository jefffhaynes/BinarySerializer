using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue124
{
    public class Issue124Tests : TestBase
    {
        [Fact]
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

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var packetData = new byte[]
                {
                    0x45, 0x00, 0x00, 0x30,
                    0x44, 0x22, 0x40, 0x00,
                    0x80, 0x06, 0x00, 0x00, 
                    0x8c, 0x7c, 0x19, 0xac,
                    0xae, 0x24, 0x1e, 0x2b
                };

            var serializer = new BinarySerialization.BinarySerializer();

            var packet = serializer.Deserialize<IPPacket>(packetData);
        }
    }
}

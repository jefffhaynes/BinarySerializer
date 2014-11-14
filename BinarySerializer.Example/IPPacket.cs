using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace BinarySerializer.Example
{
    public class Option
    {
        public int Value { get; set; }
    }

    public class IPPacket
    {
        public byte VersionAndLength { get; set; }

        public byte DscpAndEcn { get; set; }

        public ushort Length { get; set; }

        public ushort Id { get; set; }

        public ushort FlagsAndFragmentOffset { get; set; }

        public byte TTL { get; set; }

        public byte Protocol { get; set; }

        public ushort Crc { get; set; }

        public int SourceAddress { get; set; }

        public int DestinationAddress { get; set; }

        [FieldCount("HeaderLength")]
        public Option[] Options { get; set; }

        [Ignore]
        public byte Version
        {
            get { return (byte)(VersionAndLength & 0xf0 >> 4); }
            set
            {
                VersionAndLength &= 0xf0;
                VersionAndLength |= (byte) (value << 4);
            }
        }

        [Ignore]
        public byte HeaderLength
        {
            get { return(byte) (VersionAndLength & 0xf0); }
            set
            {
                VersionAndLength &= 0x0f;
                VersionAndLength |= (byte) (value & 0x0f);
            }
        }
    }
}

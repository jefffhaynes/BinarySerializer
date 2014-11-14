using System.Collections.Generic;
using BinarySerialization;

namespace Iso9660
{
    public class PathTableRecord
    {
        public byte NameLength { get; set; }
        public byte ExtendedAttributeRecordSectors { get; set; }
        public uint FirstSector { get; set; }
        public ushort ParentDirectoryRecord { get; set; }

        [FieldLength("NameLength")]
        public string Name { get; set; }

        [FieldLength("PaddingLength")]
        public byte[] Padding { get; set; }

        [Ignore]
        public int PaddingLength
        {
            get { return NameLength % 2; }
        }

        [FieldOffset("FirstSector", ConverterType = typeof(SectorByteConverter))]
        [SerializeUntil((byte)0)]
        public List<DirectoryRecord> Records { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

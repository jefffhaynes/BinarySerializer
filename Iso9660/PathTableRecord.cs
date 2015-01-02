using System.Collections.Generic;
using BinarySerialization;

namespace Iso9660
{
    public class PathTableRecord
    {
        [FieldOrder(0)]
        public byte NameLength { get; set; }

        [FieldOrder(1)]
        public byte ExtendedAttributeRecordSectors { get; set; }

        [FieldOrder(2)]
        public uint FirstSector { get; set; }

        [FieldOrder(3)]
        public ushort ParentDirectoryRecord { get; set; }

        [FieldOrder(4)]
        [FieldLength("NameLength")]
        public string Name { get; set; }

        [FieldOrder(5)]
        [FieldLength("PaddingLength")]
        public byte[] Padding { get; set; }

        [Ignore]
        public int PaddingLength
        {
            get { return NameLength % 2; }
        }

        [FieldOrder(6)]
        [FieldOffset("FirstSector", ConverterType = typeof(SectorByteConverter))]
        [SerializeUntil((byte)0)]
        public List<DirectoryRecord> Records { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

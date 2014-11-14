using System.Collections.Generic;
using System.Xml.Serialization;
using BinarySerialization;

namespace Iso9660
{
    public class PrimaryVolumeDescriptor
    {
        [FieldCount(16)]
        public List<Sector> BootArea { get; set; }

        public VolumeDescriptorType Type { get; set; }

        [FieldLength(5)]
        public string Id { get; set; }
        public byte Version { get; set; }
        public byte Unused { get; set; }

        [FieldLength(32)]
        public string SystemId { get; set; }
        [FieldLength(32)]
        public string VolumeId { get; set; }

        [FieldLength(8)]
        public byte[] Unused2 { get; set; }

        public uint SectorCount { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint SectorCountBig { get; set; }

        [FieldLength(32)]
        public byte[] Unused3 { get; set; }

        public ushort VolumeSetSize { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSetSizeBig { get; set; }


        public ushort VolumeSequenceNumber { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSequenceNumberBig { get; set; }

        public ushort SectorSize { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort SectorSizeBig { get; set; }

        public uint PathTableLength { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint PathTableLengthBig { get; set; }

        public uint FirstLittleEndianPathTableSector { get; set; }

        public uint SecondLittleEndianPathTableSector { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint FirstBigEndianPathTableSector { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint SecondBigEndianPathTableSector { get; set; }

        public DirectoryRecord RootDirectory { get; set; }

        [FieldLength(128)]
        public string VolumeSetId { get; set; }

        [FieldLength(128)]
        public string PublisherId { get; set; }

        [FieldLength(128)]
        public string DataPreparerId { get; set; }

        [FieldLength(128)]
        public string ApplicationId { get; set; }

        [FieldLength(37)]
        public string CopyrightFile { get; set; }

        [FieldLength(37)]
        public string AbstractFile { get; set; }

        [FieldLength(37)]
        public string BibliographicalFile { get; set; }

        public Iso9660DateTime VolumeCreation { get; set; }
        public Iso9660DateTime VolumeModified { get; set; }
        public Iso9660DateTime VolumeExpiration { get; set; }
        public Iso9660DateTime VolumeEffectivity { get; set; }

        public byte TrailingOne { get; set; }
        public byte TrailingZero { get; set; }

        [FieldLength(256)]
        public byte[] ApplicationReserve { get; set; }

        [FieldLength(653)]
        public byte[] Padding { get; set; }

        [Ignore]
        public long PathTableOffset
        {
            get { return FirstLittleEndianPathTableSector * SectorSize; }
        }
    }
}

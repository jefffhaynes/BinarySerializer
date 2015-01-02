using System.Collections.Generic;
using System.Xml.Serialization;
using BinarySerialization;

namespace Iso9660
{
    public class PrimaryVolumeDescriptor
    {
        [FieldOrder(0)]
        [FieldCount(16)]
        public List<Sector> BootArea { get; set; }

        [FieldOrder(1)]
        public VolumeDescriptorType Type { get; set; }

        [FieldOrder(2)]
        [FieldLength(5)]
        public string Id { get; set; }

        [FieldOrder(3)]
        public byte Version { get; set; }

        [FieldOrder(4)]
        public byte Unused { get; set; }

        [FieldOrder(5)]
        [FieldLength(32)]
        public string SystemId { get; set; }

        [FieldOrder(6)]
        [FieldLength(32)]
        public string VolumeId { get; set; }

        [FieldOrder(7)]
        [FieldLength(8)]
        public byte[] Unused2 { get; set; }

        [FieldOrder(8)]
        public uint SectorCount { get; set; }

        [XmlIgnore]
        [FieldOrder(9)]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint SectorCountBig { get; set; }

        [FieldOrder(10)]
        [FieldLength(32)]
        public byte[] Unused3 { get; set; }

        [FieldOrder(11)]
        public ushort VolumeSetSize { get; set; }

        [XmlIgnore]
        [FieldOrder(12)]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSetSizeBig { get; set; }

        [FieldOrder(13)]
        public ushort VolumeSequenceNumber { get; set; }

        [FieldOrder(14)]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSequenceNumberBig { get; set; }

        [FieldOrder(15)]
        public ushort SectorSize { get; set; }

        [XmlIgnore]
        [FieldOrder(16)]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort SectorSizeBig { get; set; }

        [FieldOrder(17)]
        public uint PathTableLength { get; set; }

        [XmlIgnore]
        [FieldOrder(18)]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint PathTableLengthBig { get; set; }

        [FieldOrder(19)]
        public uint FirstLittleEndianPathTableSector { get; set; }

        [FieldOrder(20)]
        public uint SecondLittleEndianPathTableSector { get; set; }

        [XmlIgnore]
        [FieldOrder(21)]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint FirstBigEndianPathTableSector { get; set; }

        [XmlIgnore]
        [FieldOrder(22)]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint SecondBigEndianPathTableSector { get; set; }

        [FieldOrder(23)]
        public DirectoryRecord RootDirectory { get; set; }

        [FieldOrder(24)]
        [FieldLength(128)]
        public string VolumeSetId { get; set; }

        [FieldOrder(25)]
        [FieldLength(128)]
        public string PublisherId { get; set; }

        [FieldOrder(26)]
        [FieldLength(128)]
        public string DataPreparerId { get; set; }

        [FieldOrder(27)]
        [FieldLength(128)]
        public string ApplicationId { get; set; }

        [FieldOrder(28)]
        [FieldLength(37)]
        public string CopyrightFile { get; set; }

        [FieldOrder(29)]
        [FieldLength(37)]
        public string AbstractFile { get; set; }

        [FieldOrder(30)]
        [FieldLength(37)]
        public string BibliographicalFile { get; set; }

        [FieldOrder(31)]
        public Iso9660DateTime VolumeCreation { get; set; }
        [FieldOrder(32)]
        public Iso9660DateTime VolumeModified { get; set; }
        [FieldOrder(33)]
        public Iso9660DateTime VolumeExpiration { get; set; }
        [FieldOrder(34)]
        public Iso9660DateTime VolumeEffectivity { get; set; }

        [FieldOrder(35)]
        public byte TrailingOne { get; set; }
        [FieldOrder(36)]
        public byte TrailingZero { get; set; }

        [FieldOrder(37)]
        [FieldLength(256)]
        public byte[] ApplicationReserve { get; set; }

        [FieldOrder(38)]
        [FieldLength(653)]
        public byte[] Padding { get; set; }

        [Ignore]
        public long PathTableOffset
        {
            get { return FirstLittleEndianPathTableSector * SectorSize; }
        }
    }
}

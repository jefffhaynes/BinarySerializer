using System;
using System.IO;
using System.Xml.Serialization;
using BinarySerialization;

namespace Iso9660
{
    public class DirectoryRecord
    {
        private const int PrePaddingLength = 33;

        public byte Length { get; set; }
        public byte ExtendedAttributeRecordSectors { get; set; }

        public uint FirstSectorData { get; set; }
        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint FirstSectorDataBig { get; set; }

        public uint DataLength { get; set; }
        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint DataLengthBig { get; set; }

        [XmlIgnore]
        public byte YearsSince1900 { get; set; }
        [XmlIgnore]
        public byte Month { get; set; }
        [XmlIgnore]
        public byte Day { get; set; }
        [XmlIgnore]
        public byte Hour { get; set; }
        [XmlIgnore]
        public byte Minute { get; set; }
        [XmlIgnore]
        public byte Second { get; set; }
        [XmlIgnore]
        public sbyte GmtOffset { get; set; }
        public RecordType RecordType { get; set; }
        public byte InterleaveFileUnitSize { get; set; }
        public byte InterleaveGapSize { get; set; }
        public ushort VolumeSequenceNumber { get; set; }

        [XmlIgnore]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSequenceNumberBig { get; set; }
        public byte IdentifierLength { get; set; }

        [FieldLength("IdentifierLength")]
        public string Identifier { get; set; }

        [FieldLength("PaddingLength")]
        public byte[] Padding { get; set; }

        [Ignore]
        public int PaddingLength 
        {
            get { return (IdentifierLength + 1) % 2; } 
        }

        [Ignore]
        public DateTime DateTime
        {
            get
            {
                return new DateTime(1900 + YearsSince1900, Month, Day, Hour, Minute, Second, DateTimeKind.Local);
            }

            set
            {
                YearsSince1900 = (byte)(value.Year - 1900);
                Month = (byte)(value.Month);
                Day = (byte)(value.Day);
                Hour = (byte)(value.Hour);
                Minute = (byte)(value.Minute);
                Second = (byte)(value.Second);
            }
        }

        [Ignore]
        public int SystemReserveLength
        {
            get { return Length - PrePaddingLength - IdentifierLength - PaddingLength; }
        }

        [FieldLength("SystemReserveLength")]
        public byte[] SystemReserve { get; set; }

        [XmlIgnore]
        [SerializeWhen("RecordType", RecordType.File)]
        [FieldOffset("FirstSectorData", ConverterType = typeof(SectorByteConverter))]
        [FieldLength("DataLength")]
        public Stream Data { get; set; }

        public override string ToString()
        {
            return Identifier;
        }
    }
}

using System;
using System.IO;
using System.Xml.Serialization;
using BinarySerialization;

namespace Iso9660
{
    public class DirectoryRecord
    {
        private const int PrePaddingLength = 33;

        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        public byte ExtendedAttributeRecordSectors { get; set; }

        [FieldOrder(2)]
        public uint FirstSectorData { get; set; }

        [XmlIgnore]
        [FieldOrder(3)]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint FirstSectorDataBig { get; set; }

        [FieldOrder(4)]
        public uint DataLength { get; set; }

        [XmlIgnore]
        [FieldOrder(5)]
        [SerializeAs(Endianness = Endianness.Big)]
        public uint DataLengthBig { get; set; }

        [XmlIgnore]
        [FieldOrder(6)]
        public byte YearsSince1900 { get; set; }

        [XmlIgnore]
        [FieldOrder(7)]
        public byte Month { get; set; }

        [XmlIgnore]
        [FieldOrder(8)]
        public byte Day { get; set; }

        [XmlIgnore]
        [FieldOrder(9)]
        public byte Hour { get; set; }

        [XmlIgnore]
        [FieldOrder(10)]
        public byte Minute { get; set; }

        [XmlIgnore]
        [FieldOrder(11)]
        public byte Second { get; set; }

        [XmlIgnore]
        [FieldOrder(12)]
        public sbyte GmtOffset { get; set; }

        [FieldOrder(13)]
        public RecordType RecordType { get; set; }

        [FieldOrder(14)]
        public byte InterleaveFileUnitSize { get; set; }

        [FieldOrder(15)]
        public byte InterleaveGapSize { get; set; }

        [FieldOrder(16)]
        public ushort VolumeSequenceNumber { get; set; }

        [XmlIgnore]
        [FieldOrder(17)]
        [SerializeAs(Endianness = Endianness.Big)]
        public ushort VolumeSequenceNumberBig { get; set; }

        [FieldOrder(18)]
        public byte IdentifierLength { get; set; }

        [FieldOrder(19)]
        [FieldLength("IdentifierLength")]
        public string Identifier { get; set; }

        [FieldOrder(20)]
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

        [FieldOrder(21)]
        [FieldLength("SystemReserveLength")]
        public byte[] SystemReserve { get; set; }

        [XmlIgnore]
        [FieldOrder(22)]
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

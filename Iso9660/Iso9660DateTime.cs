using System;
using System.Xml.Serialization;
using BinarySerialization;

namespace Iso9660
{
    public class Iso9660DateTime
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        [SerializeAs(SerializedType.SizedString)]
        public uint Year { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        [SerializeAs(SerializedType.SizedString)]
        public ushort Month { get; set; }

        [FieldOrder(2)]
        [FieldLength(2)]
        [SerializeAs(SerializedType.SizedString)]
        public ushort Day { get; set; }

        [FieldOrder(3)]
        [FieldLength(2)]
        [SerializeAs(SerializedType.SizedString)]
        public ushort Hour { get; set; }

        [FieldOrder(4)]
        [FieldLength(2)]
        [SerializeAs(SerializedType.SizedString)]
        public ushort Minute { get; set; }

        [FieldOrder(5)]
        [FieldLength(2)]
        [SerializeAs(SerializedType.SizedString)]
        public ushort Second { get; set; }

        [FieldOrder(6)]
        [FieldLength(2)]
        [SerializeAs(SerializedType.SizedString)]
        public ushort Hundredth { get; set; }

        [FieldOrder(7)]
        public sbyte GmtOffset { get; set; }

        [Ignore]
        [XmlIgnore]
        public DateTime DateTime
        {
            get
            {
                return (new DateTime((int)Year, Month, Day, Hour, Minute, Second)).AddMilliseconds(Hundredth * 10);
            }

            set
            {
                Year = (uint)value.Year;
                Month = (ushort)value.Month;
                Day = (ushort)value.Day;
                Hour = (ushort)value.Hour;
                Minute = (ushort)value.Minute;
                Second = (ushort)value.Second;
                Hundredth = (ushort)(value.Millisecond / 10);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Performance
{
    [Serializable]
    public class Beer
    {
        [SerializeAs(SerializedType.LengthPrefixedString)]
        [FieldOrder(0)] public string Brand;

        [NonSerialized] [FieldOrder(1)] public byte SortCount;

        [FieldOrder(2)]
        [FieldCount("SortCount", ConverterType = typeof(TwiceConverter))]
        [FieldCrc16("Crc")]
        public List<SortContainer> Sort;

        [FieldOrder(3)] public float Alcohol;

        [SerializeAs(SerializedType.LengthPrefixedString)]
        [FieldOrder(4)] public string Brewery;

        [FieldOrder(5)]
        public ushort Crc { get; set; }

        [FieldOrder(6)]
        [FieldBitLength(5)]
        public byte WeirdNumber { get; set; }

        [FieldOrder(7)]
        [FieldBitLength(3)]
        public byte WeirdNumber2 { get; set; }

        [FieldOrder(8)]
        [SerializeAs(SerializedType.TerminatedString, StringTerminator = (char)5)]
        public string TerminatedString { get; set; }

        [FieldOrder(9)]
        public Color Color { get; set; }
    }

    [Serializable]
    public class SortContainer
    {
        [NonSerialized]
        [FieldOrder(0)]
        public byte NameLength;

        [FieldOrder(1)]
        [FieldLength("NameLength")]
        public string Name;
    }
}

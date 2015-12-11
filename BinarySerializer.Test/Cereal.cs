﻿using System.Collections.Generic;
using System.IO;
using BinarySerialization;

namespace BinarySerialization.Test
{
    public class Cereal
    {
        [FieldOrder(0)]
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string IsLittleEndian { get; set; }

        [FieldOrder(1)]
        [FieldLength(6)]
        public string Name { get; set; }

        [FieldOrder(2)]
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Manufacturer { get; set; }

        [FieldOrder(3)]
        public ushort OtherStuffCount { get; set; }

        [FieldOrder(4)]
        [FieldOffset(10000)]
        public double Outlier { get; set; }

        [FieldOrder(5)]
        public NutritionalInformation NutritionalInformation { get; set; }

        [FieldOrder(6)]
        public double DoubleField;

        [FieldOrder(7)]
        [FieldCount("OtherStuffCount")]
        [SerializeAs(SerializedType.SizedString)]
        [ItemLength(3)]
        public List<string> OtherStuff { get; set; }

        [FieldOrder(8)]
        [FieldLength(4)]
        [SerializeAs(SerializedType.SizedString)]
        public CerealShape Shape { get; set; }

        [FieldOrder(9)]
        public CerealShape DefinitelyNotTheShape { get; set; }

        [FieldOrder(10)]
        [SerializeWhen("Shape", "Square")]
        public string DontSerializeMe { get; set; }

        [FieldOrder(11)]
        [SerializeWhen("Shape", CerealShape.Circular)]
        public string SerializeMe { get; set; }

        [FieldOrder(12)]
        [SerializeUntil((byte)0)]
        public List<string> ExplicitlyTerminatedList { get; set; }

        [FieldOrder(13)]
        [FieldLength(9)]
        public List<CerealShape> ImplicitlyTerminatedList { get; set; }

        [FieldOrder(14)]
        [FieldCount(3)]
        public int[] ArrayOfInts { get; set; }

        [FieldOrder(15)]
        [FieldLength]
        public string InvalidFieldLength { get; set; }

        [FieldOrder(16)]
        public long DisclaimerLength { get; set; }

        [FieldOrder(17)]
        [FieldLength("DisclaimerLength")]
        public Stream Disclaimer { get; set; }
    }
}

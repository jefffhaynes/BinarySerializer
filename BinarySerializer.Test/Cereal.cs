using System.Collections.Generic;
using System.IO;
using BinarySerialization;

namespace BinarySerializer.Test
{
    public class Cereal
    {
        [SerializeAs(SerializedType.NullTerminatedString, Order=0)]
        public string IsLittleEndian { get; set; }

        [FieldLength(6)]
        public string Name { get; set; }

        [SerializeAs(SerializedType.NullTerminatedString, Order = 0)]
        public string Manufacturer { get; set; }

        public ushort OtherStuffCount { get; set; }

        [FieldOffset(1000)]
        public double Outlier { get; set; }

        public NutritionalInformation NutritionalInformation { get; set; }

        public double DoubleField;

        [FieldCount("OtherStuffCount")]
        [SerializeAs(SerializedType.SizedString)]
        [ItemLength(3)]
        public List<string> OtherStuff { get; set; }

        [FieldLength(4)]
        [SerializeAs(SerializedType.SizedString)]
        public CerealShape Shape { get; set; }

        public CerealShape DefinitelyNotTheShape { get; set; }

        [SerializeWhen("Shape", CerealShape.Square)]
        public string DontSerializeMe { get; set; }

        [SerializeWhen("Shape", CerealShape.Circular)]
        public string SerializeMe { get; set; }

        [SerializeUntil((byte)0)]
        public List<string> ExplicitlyTerminatedList { get; set; }

        [FieldLength(12)]
        public List<CerealShape> ImplicitlyTerminatedList { get; set; }

        [FieldCount(3)]
        public int[] ArrayOfInts { get; set; }

        [FieldLength]
        public string InvalidFieldLength { get; set; }
        
        public long DisclaimerLength { get; set; }

        [FieldLength("DisclaimerLength")]
        public Stream Disclaimer { get; set; }
    }
}

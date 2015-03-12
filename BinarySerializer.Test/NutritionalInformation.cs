using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerialization.Test
{
    public class NutritionalInformation
    {
        [FieldOrder(0)]
        public int Calories { get; set; }

        [FieldOrder(1)]
        public float Fat { get; set; }

        [FieldOrder(2)]
        public ushort Cholesterol { get; set; }

        [FieldOrder(4)]
        public ushort VitaminA { get; set; }

        [FieldOrder(3)]
        public uint VitaminB { get; set; }

        [FieldOrder(5)]
        [FieldCount("OtherStuffCount", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(Cereal))]
        public List<string> OtherNestedStuff { get; set; }

        [FieldOrder(6)]
        [FieldCount("OtherStuffCount", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorLevel = 2)]
        public List<string> OtherNestedStuff2 { get; set; }

        [FieldOrder(7)]
        [ItemSerializeUntil("Last", true)]
        public List<Toy> Toys { get; set; }

        [FieldOrder(8)]
        [FieldLength("Outlier", ConverterType = typeof(DoubleOutlierConverter), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorLevel = 2)]
        public string WeirdOutlierLengthedField { get; set; }

        [FieldOrder(9)]
        public Ingredients Ingredients { get; set; }
    }
}

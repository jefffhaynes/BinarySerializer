using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test
{
    public class NutritionalInformation
    {
        public int Calories { get; set; }
        public float Fat { get; set; }
        public ushort Cholesterol { get; set; }

        [SerializeAs(Order=1)]
        public ushort VitaminA { get; set; }

        [SerializeAs(Order=0)]
        public uint VitaminB { get; set; }

        [FieldCount("OtherStuffCount", Mode = RelativeSourceMode.FindAncestor, AncestorType = typeof(Cereal))]
        public List<string> OtherNestedStuff { get; set; }

        [FieldCount("OtherStuffCount", Mode = RelativeSourceMode.FindAncestor, AncestorLevel = 2)]
        public List<string> OtherNestedStuff2 { get; set; }

        [ItemSerializeUntil("Last", true)]
        public List<Toy> Toys { get; set; }

        [FieldLength("Outlier", ConverterType = typeof(DoubleOutlierConverter), Mode = RelativeSourceMode.FindAncestor, AncestorLevel = 2)]
        public string WeirdOutlierLengthedField { get; set; }

        public Ingredients Ingredients { get; set; }
    }
}

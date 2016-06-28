namespace BinarySerialization.Test.Subtype
{
    public class NonUniqueSubtypeValuesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof(SubclassA))]
        [Subtype("Indicator", 1, typeof(SubclassB))]
        public Superclass Superclass { get; set; }
    }
}

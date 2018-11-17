namespace BinarySerialization.Test.Subtype
{
    public class NonUniqueSubtypesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof (SubclassA))]
        [Subtype("Indicator", 2, typeof (SubclassA))]
        public Superclass Superclass { get; set; }
    }
}
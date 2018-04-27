namespace BinarySerialization.Test.Subtype
{
    public class NonUniqueSubtypesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(Indicator), 1, typeof (SubclassA))]
        [Subtype(nameof(Indicator), 2, typeof (SubclassA))]
        public Superclass Superclass { get; set; }
    }
}
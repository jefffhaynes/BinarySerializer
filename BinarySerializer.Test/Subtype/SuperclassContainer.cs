namespace BinarySerialization.Test.Subtype
{
    public class SuperclassContainer
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        public byte Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(nameof(Length))]
        [Subtype(nameof(Indicator), 1, typeof (SubclassA))]
        [Subtype(nameof(Indicator), 2, typeof (SubclassB))]
        [Subtype(nameof(Indicator), 3, typeof (SubSubclassC))]
        public Superclass Value { get; set; }
    }
}
namespace BinarySerialization.Test.Subtype
{
    public class SuperclassContainer
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        public byte Length { get; set; }

        [FieldOrder(2)]
        [FieldLength("Length")]
        [Subtype("Indicator", 1, typeof (SubclassA))]
        [Subtype("Indicator", 2, typeof (SubclassB))]
        [Subtype("Indicator", 3, typeof (SubSubclassC))]
        public Superclass Value { get; set; }
    }
}
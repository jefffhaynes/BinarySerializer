namespace BinarySerialization.Test.Subtype
{
    public class MultipleBindingModesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof(SubclassA))]
        [Subtype("Indicator", 1, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        public Superclass Value { get; set; }
    }
}

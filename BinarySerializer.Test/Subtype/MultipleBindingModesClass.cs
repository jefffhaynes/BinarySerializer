namespace BinarySerialization.Test.Subtype
{
    public class MultipleBindingModesClass
    {
        public byte Indicator { get; set; }

        [Subtype("Indicator", 1, typeof(SubclassA))]
        [Subtype("Indicator", 1, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        public Superclass Value { get; set; }
    }
}

namespace BinarySerialization.Test.Subtype
{
    public class MixedBindingModesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        [Subtype("Indicator", 1, typeof(SubclassA))]
        [Subtype("Indicator", 2, typeof(SubclassB), BindingMode = BindingMode.OneWayToSource)]
        public Superclass Value { get; set; }
    }
}

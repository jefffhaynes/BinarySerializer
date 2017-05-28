namespace BinarySerialization.Test.Subtype
{
    public class MixedBindingModesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        [Subtype("Indicator", 4, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        [Subtype("Indicator", 2, typeof(SubclassA), BindingMode = BindingMode.OneWay)]
        [Subtype("Indicator", 2, typeof(SubclassB), BindingMode = BindingMode.OneWayToSource)]
        [Subtype("Indicator", 3, typeof(SubSubclassC))]
        public Superclass Value { get; set; }
    }
}

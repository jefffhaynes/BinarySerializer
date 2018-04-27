namespace BinarySerialization.Test.Subtype
{
    public class MixedBindingModesClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(Indicator), 1, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(Indicator), 4, typeof(SubclassB), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(Indicator), 2, typeof(SubclassA), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(Indicator), 2, typeof(SubclassB), BindingMode = BindingMode.OneWayToSource)]
        [Subtype(nameof(Indicator), 3, typeof(SubSubclassC))]
        public Superclass Value { get; set; }
    }
}

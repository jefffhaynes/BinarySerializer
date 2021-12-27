namespace BinarySerialization.Test.Subtype;

public class InvalidDefaultSubtypeContainerClass
{
    [FieldOrder(0)]
    public byte Indicator { get; set; }

    [FieldOrder(1)]
    [Subtype(nameof(Indicator), 1, typeof(SubclassA))]
    [Subtype(nameof(Indicator), 2, typeof(SubclassB))]
    [Subtype(nameof(Indicator), 3, typeof(SubSubclassC))]
    [SubtypeDefault(typeof(string))]
    public Superclass Value { get; set; }
}

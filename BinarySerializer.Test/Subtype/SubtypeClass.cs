namespace BinarySerialization.Test.Subtype;

public class SubtypeClass
{
    [FieldOrder(0)]
    public SubclassType Subtype { get; set; }

    [FieldOrder(1)]
    [Subtype(nameof(Subtype), SubclassType.A, typeof(SubclassA))]
    [Subtype(nameof(Subtype), SubclassType.B, typeof(SubclassB))]
    [Subtype(nameof(Subtype), SubclassType.C, typeof(SubSubclassC))]
    public Superclass Field { get; set; }

    [FieldOrder(2)]
    public string SubtypeString { get; set; }

    [FieldOrder(3)]
    [Subtype(nameof(SubtypeString), nameof(SubclassType.A), typeof(SubclassA))]
    [Subtype(nameof(SubtypeString), nameof(SubclassType.B), typeof(SubclassB))]
    [Subtype(nameof(SubtypeString), nameof(SubclassType.C), typeof(SubSubclassC))]
    public Superclass Field2 { get; set; }
}

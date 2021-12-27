namespace BinarySerialization.Test.Subtype;

public class IncompleteSubtypeClass
{
    [FieldOrder(0)]
    public SubclassType Subtype { get; set; }

    [FieldOrder(1)]
    [Subtype(nameof(Subtype), SubclassType.A, typeof(SubclassA))]
    public Superclass Field { get; set; }
}

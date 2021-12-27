namespace BinarySerialization.Test.Subtype;

public class SubtypeDefaultOnlyClass
{
    [FieldOrder(0)]
    public byte Key { get; set; }

    [FieldOrder(1)]
    [SubtypeDefault(typeof(DefaultSubtypeClass))]
    public Superclass Value { get; set; }
}

namespace BinarySerialization.Test.Subtype;

public class SubtypeMixedClass
{
    [FieldOrder(0)]
    public int Key { get; set; }

    [FieldOrder(1)]
    [Subtype(nameof(Key), 3, typeof(SubSubclassC))]
    [SubtypeFactory(nameof(Key), typeof(SubtypeFactory))]
    public Superclass Value { get; set; }
}

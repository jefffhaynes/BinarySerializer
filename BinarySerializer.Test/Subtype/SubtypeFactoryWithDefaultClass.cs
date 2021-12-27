namespace BinarySerialization.Test.Subtype;

public class SubtypeFactoryWithDefaultClass
{
    [FieldOrder(0)]
    public byte Key { get; set; }

    [FieldOrder(1)]
    [SubtypeFactory(nameof(Key), typeof(SubtypeFactory))]
    [SubtypeDefault(typeof(DefaultSubtypeClass))]
    public Superclass Value { get; set; }
}

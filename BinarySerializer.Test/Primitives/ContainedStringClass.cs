namespace BinarySerialization.Test.Primitives;

public class ContainedStringClass
{
    [FieldOrder(0)]
    public string Value { get; set; }

    [FieldOrder(1)]
    public int Number { get; set; }
}

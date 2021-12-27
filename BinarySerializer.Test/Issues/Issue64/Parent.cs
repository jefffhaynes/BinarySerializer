namespace BinarySerialization.Test.Issues.Issue64;

public class Parent
{
    [FieldOrder(0)]
    public int Value { get; set; }

    [FieldOrder(1)]
    public Entry Entry { get; set; }
}

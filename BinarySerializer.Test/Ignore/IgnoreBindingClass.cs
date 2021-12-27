namespace BinarySerialization.Test.Ignore;

public class IgnoreBindingClass
{
    [Ignore]
    public int Length => 5;

    [FieldLength("Length")]
    public string Value { get; set; }
}

namespace BinarySerialization.Test.Subtype;

public class IgnoredSubtype : Superclass
{
    [Ignore]
    public int A { get; set; }

    [Ignore]
    public int B { get; set; }
}

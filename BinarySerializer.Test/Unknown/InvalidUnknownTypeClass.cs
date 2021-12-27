namespace BinarySerialization.Test.Unknown;

public class InvalidUnknownTypeClass
{
    [Subtype("", 0, typeof(int))]
    public object Field { get; set; }
}

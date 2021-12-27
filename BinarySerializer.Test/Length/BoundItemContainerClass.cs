namespace BinarySerialization.Test.Length;

public class BoundItemContainerClass
{
    [FieldOrder(0)]
    public int NameLength { get; set; }

    [FieldOrder(1)]
    public List<BoundItemClass> Items { get; set; }
}

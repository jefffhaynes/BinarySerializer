namespace BinarySerialization.Test.Count;

public class PrimitiveArrayBindingClass
{
    [FieldOrder(0)]
    public int ItemCount { get; set; }

    [FieldOrder(1)]
    [FieldCount(nameof(ItemCount))]
    public int[] Ints { get; set; }
}

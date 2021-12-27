namespace BinarySerialization.Test.ItemSubtype;

public class ItemSubtypeFactoryClass
{
    [FieldOrder(0)]
    public byte Key { get; set; }

    [FieldOrder(1)]
    [ItemSubtypeFactory(nameof(Key), typeof(ItemSubtypeFactory))]
    public List<IItemSubtype> Value { get; set; }
}

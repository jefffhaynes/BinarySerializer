namespace BinarySerialization.Test.ItemSubtype;

public class ItemSubtypeFactoryWithDefaultClass
{
    [FieldOrder(0)]
    public byte Key { get; set; }

    [FieldOrder(1)]
    [ItemSubtypeFactory(nameof(Key), typeof(ItemSubtypeFactory))]
    [ItemSubtypeDefault(typeof(DefaultItemType))]
    public List<IItemSubtype> Items { get; set; }
}

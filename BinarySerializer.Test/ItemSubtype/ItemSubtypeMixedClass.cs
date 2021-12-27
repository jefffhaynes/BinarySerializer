namespace BinarySerialization.Test.ItemSubtype;

public class ItemSubtypeMixedClass
{
    [FieldOrder(0)]
    public byte Key { get; set; }

    [FieldOrder(1)]
    [ItemSubtype(nameof(Key), 2, typeof(ItemTypeB))]
    [ItemSubtypeFactory(nameof(Key), typeof(ItemSubtypeFactory))]
    public List<IItemSubtype> Value { get; set; }
}

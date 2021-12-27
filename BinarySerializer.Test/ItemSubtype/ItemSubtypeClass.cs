namespace BinarySerialization.Test.ItemSubtype;

public class ItemSubtypeClass
{
    [FieldOrder(0)]
    public byte Indicator { get; set; }

    [FieldOrder(1)]
    [ItemSubtype(nameof(Indicator), 1, typeof(ItemTypeA))]
    [ItemSubtype(nameof(Indicator), 2, typeof(ItemTypeB))]
    [ItemSubtype(nameof(Indicator), 3, typeof(CustomItem))]
    [ItemSubtypeDefault(typeof(DefaultItemType))]
    public List<IItemSubtype> Items { get; set; }
}

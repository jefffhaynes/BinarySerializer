namespace BinarySerialization.Test.ItemLength
{
    public class ArrayItemBoundLengthClass
    {
        [FieldOrder(0)]
        public int ItemLength { get; set; }

        [FieldOrder(1)]
        [ItemLength("ItemLength")]
        public string[] Items { get; set; }
    }
}
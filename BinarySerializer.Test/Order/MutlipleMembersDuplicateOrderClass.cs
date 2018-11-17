namespace BinarySerialization.Test.Order
{
    public class MutlipleMembersDuplicateOrderClass
    {
        [FieldOrder(3)]
        public int First { get; set; }

        [FieldOrder(3)]
        public int Second { get; set; }
    }
}
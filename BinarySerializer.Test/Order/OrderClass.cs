namespace BinarySerialization.Test.Order
{
    public class OrderClass
    {
        [FieldOrder(3)]
        [FieldLength("NameLength")]
        public string Name { get; set; }

        [FieldOrder(2)]
        public byte NameLength { get; set; }

        [FieldOrder(1)]
        public byte Second { get; set; }

        [FieldOrder(0)]
        public byte First { get; set; }
    }
}
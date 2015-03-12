using BinarySerialization;

namespace BinarySerialization.Test.Order
{
    public class OrderClass
    {
        [FieldOrder(1)]
        public byte Second { get; set; }

        [FieldOrder(0)]
        public byte First { get; set; }
    }
}

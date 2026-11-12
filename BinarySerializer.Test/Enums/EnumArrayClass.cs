namespace BinarySerialization.Test.Enums
{
    public class EnumArrayClass
    {
        [FieldOrder(0)]
        public byte Count { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public ColorValues[] Colors { get; set; }
    }
}

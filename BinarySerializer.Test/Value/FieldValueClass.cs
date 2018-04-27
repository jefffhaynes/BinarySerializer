namespace BinarySerialization.Test.Value
{
    public class FieldValueClass
    {
        [FieldOrder(0)]
        [FieldValue(nameof(ValueCopy))]
        public int Value { get; set; }

        [FieldOrder(1)]
        public int ValueCopy { get; set; }
    }
}
namespace BinarySerialization.Test.Value
{
    public class FieldValueClass
    {
        [FieldOrder(0)]
        [FieldValue("ValueCopy")]
        public int Value { get; set; }

        [FieldOrder(1)]
        public int ValueCopy { get; set; }
    }
}
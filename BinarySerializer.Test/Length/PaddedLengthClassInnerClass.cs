namespace BinarySerialization.Test.Length
{
    public class PaddedLengthClassInnerClass
    {
        [FieldOrder(0)]
        public byte ValueLength { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(ValueLength))]
        public string Value { get; set; }
    }
}
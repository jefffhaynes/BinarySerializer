namespace BinarySerialization.Test.Length
{
    public class PaddedLengthClassClass
    {
        [FieldOrder(0)]
        [FieldLength(20)]
        public PaddedLengthClassInnerClass InnerClass { get; set; }

        [FieldOrder(1)]
        [FieldLength(20)]
        public PaddedLengthClassInnerClass InnerClass2 { get; set; }
    }
}
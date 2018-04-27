namespace BinarySerialization.Test.Misc
{
    public class DontFlushTooMuchClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length))]
        public string Value { get; set; }

        [FieldOrder(2)]
        public int Length2 { get; set; }

        [FieldOrder(3)]
        public DontFlushTooMuchInternalClass Internal { get; set; }
    }
}

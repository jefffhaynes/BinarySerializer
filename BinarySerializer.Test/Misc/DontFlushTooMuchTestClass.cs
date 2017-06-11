namespace BinarySerialization.Test.Misc
{
    public class DontFlushTooMuchTestClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public string Value { get; set; }
    }
}

namespace BinarySerialization.Test.Events
{
    public class EventTestInnerClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public EventTestInnerInnerClass InnerClass { get; set; }
    }
}
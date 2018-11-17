namespace BinarySerialization.Test.Events
{
    public class EventTestClass
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public EventTestInnerClass InnerClass { get; set; }
    }
}
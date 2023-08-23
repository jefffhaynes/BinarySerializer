namespace BinarySerialization.Test.Issues.Issue225
{
    public class ValueDescriptorFloatValue
    {
        [FieldOrder(0)]
        public ValueDescriptor ValueDescriptor { get; set; }
        [FieldOrder(1)]
        public float Value { get; set; }
    }
}
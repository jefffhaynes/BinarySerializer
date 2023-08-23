namespace BinarySerialization.Test.Issues.Issue225
{
    public class ValueDescriptorDoubleValue
    {
        [FieldOrder(0)]
        public ValueDescriptor ValueDescriptor { get; set; }
        [FieldOrder(1)]
        public double Value { get; set; }
    }
}
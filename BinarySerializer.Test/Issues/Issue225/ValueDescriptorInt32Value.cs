using System;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class ValueDescriptorInt32Value
    {
        [FieldOrder(0)]
        public ValueDescriptor ValueDescriptor { get; set; }
        [FieldOrder(1)]
        public Int32 Value { get; set; }
    }
}
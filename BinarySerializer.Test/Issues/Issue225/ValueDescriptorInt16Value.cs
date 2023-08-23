using System;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class ValueDescriptorInt16Value
    {
        [FieldOrder(0)]
        public ValueDescriptor ValueDescriptor { get; set; }
        [FieldOrder(1)]
        public Int16 Value { get; set; }
    }
}
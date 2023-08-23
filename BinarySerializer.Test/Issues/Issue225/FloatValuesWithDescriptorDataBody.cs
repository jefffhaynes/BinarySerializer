using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class FloatValuesWithDescriptorDataBody : ValueWithDescriptorDataBlock
    {
        public List<ValueDescriptorFloatValue> Data { get; set; }
    }
}
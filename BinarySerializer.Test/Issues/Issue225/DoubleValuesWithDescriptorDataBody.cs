using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class DoubleValuesWithDescriptorDataBody : ValueWithDescriptorDataBlock
    {
        public List<ValueDescriptorDoubleValue> Data { get; set; }
    }
}
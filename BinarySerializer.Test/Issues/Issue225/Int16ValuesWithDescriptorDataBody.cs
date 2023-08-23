using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class Int16ValuesWithDescriptorDataBody : ValueWithDescriptorDataBlock
    {
        public List<ValueDescriptorInt16Value> Data { get; set; }
    }
}
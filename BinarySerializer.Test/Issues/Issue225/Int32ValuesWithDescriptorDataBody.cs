using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class Int32ValuesWithDescriptorDataBody : ValueWithDescriptorDataBlock
    {
        public List<ValueDescriptorInt32Value> Data { get; set; }
    }
}
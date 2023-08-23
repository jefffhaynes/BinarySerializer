using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class FloatPlainValuesDataBody : PlainValueDataBlock
    {
        public List<float> Data { get; set; }
    }
}
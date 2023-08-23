using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class DoublePlainValuesDataBody : PlainValueDataBlock
    {
        public List<double> Data { get; set; }
    }
}
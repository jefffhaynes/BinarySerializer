using System;
using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class Int32PlainValuesDataBody : PlainValueDataBlock
    {
        public List<Int32> Data { get; set; }
    }
}
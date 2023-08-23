using System;
using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class Int16PlainValuesDataBody : PlainValueDataBlock
    {
        public List<Int16> Data { get; set; }
    }
}
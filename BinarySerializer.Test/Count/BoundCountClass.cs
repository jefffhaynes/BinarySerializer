using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test.Count
{
    public class BoundCountClass
    {
        public ushort FieldCountField { get; set; }

        [FieldCount("FieldCountField")]
        public List<string> Field { get; set; }
    }
}
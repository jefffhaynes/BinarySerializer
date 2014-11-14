using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test.Count
{
    public class ConstCountClass
    {
        [FieldCount(3)]
        public List<string> Field { get; set; }
    }
}
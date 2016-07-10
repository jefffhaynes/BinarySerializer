using System.Collections.Generic;

namespace BinarySerialization.Test.Length
{
    public class ConstCollectionLengthClass
    {
        [FieldLength(6)]
        public List<string> Field { get; set; }
    }
}
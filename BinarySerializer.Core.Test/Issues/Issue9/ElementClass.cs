using System;

namespace BinarySerialization.Test.Issues.Issue9
{
    public class ElementClass
    {
        [FieldOrder(0)]
        public Int32 B_ID { get; set; }

        [FieldOrder(1)]
        [FieldLength(51)]
        public Entry B_Name { get; set; } 
    }
}

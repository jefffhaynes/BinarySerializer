using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test.Until
{
    public class UntilTestClass
    {
        [FieldOrder(0)]
        [SerializeUntil(0)]
        public List<string> Items { get; set; }

        [FieldOrder(1)]
        public string AfterItems { get; set; }
    }
}

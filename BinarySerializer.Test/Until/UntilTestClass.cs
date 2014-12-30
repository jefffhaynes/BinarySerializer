using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Test.Until
{
    public class UntilTestClass
    {
        [SerializeUntil(0)]
        public List<string> Items { get; set; }

        public string AfterItems { get; set; }
    }
}

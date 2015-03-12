using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerialization.Test.Until
{
    public class UntilTestClass<TItem>
    {
        [FieldOrder(0)]
        [SerializeUntil(0)]
        public List<TItem> Items { get; set; }

        [FieldOrder(1)]
        public string AfterItems { get; set; }
    }
}

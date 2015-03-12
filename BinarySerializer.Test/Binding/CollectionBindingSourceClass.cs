using System.Collections.Generic;

namespace BinarySerialization.Test.Binding
{
    public class CollectionBindingSourceClass
    {
        [FieldOrder(0)]
        public List<int> List { get; set; }

        [FieldOrder(1)]
        [FieldLength("List")]
        public string Name { get; set; }
    }
}

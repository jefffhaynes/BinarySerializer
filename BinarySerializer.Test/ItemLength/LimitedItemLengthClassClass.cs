using System.Collections.Generic;

namespace BinarySerializer.Test.ItemLength
{
    public class LimitedItemLengthClassClass
    {
        [BinarySerialization.ItemLength(3)]
        public List<LimitedItemLengthClassInnerClass> InnerClasses { get; set; }
    }
}

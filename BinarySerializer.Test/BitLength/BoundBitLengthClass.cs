using System.Collections.Generic;

namespace BinarySerialization.Test.BitLength
{
    public class BoundBitLengthClass
    {
        [FieldOrder(0)]
        public int BitLength { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(nameof(BitLength))]
        public List<InternalBitLengthClass> Items { get; set; }
    }
}

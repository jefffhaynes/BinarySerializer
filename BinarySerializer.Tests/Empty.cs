
using BinarySerialization;

namespace BinarySerializer.Test
{
    public class Empty
    {
        [FieldLength(24)]
        public byte[] Filler { get; set; }
    }
}

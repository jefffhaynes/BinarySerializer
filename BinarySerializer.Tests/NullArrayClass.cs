
using BinarySerialization;

namespace BinarySerialization.Test
{
    public class NullArrayClass
    {
        [FieldLength(24)]
        public byte[] Filler { get; set; }

        public int LastMember;
    }
}

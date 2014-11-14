using BinarySerialization;

namespace Iso9660
{
    public class Sector
    {
        [FieldLength(2048)]
        public byte[] Filler { get; set; }
    }
}

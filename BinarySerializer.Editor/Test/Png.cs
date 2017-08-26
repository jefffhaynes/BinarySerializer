using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.Editor.Test
{
    public class Png
    {
        [FieldOrder(0)]
        [FieldLength(8)]
        public byte[] FileHeader { get; set; }

        [FieldOrder(1)]
        public List<PngChunkContainer> Chunks { get; set; }
    }
}

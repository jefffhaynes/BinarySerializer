using System.Collections.Generic;

namespace BinarySerializer.Editor.Test
{
    public class PngPaletteChunk : PngChunk
    {
        public List<PngRgb> PaletteEntry { get; set; }
    }
}

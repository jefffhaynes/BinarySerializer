using BinarySerialization;

namespace BinarySerializer.Editor.Test
{
    public class PngImageHeaderChunk : PngChunk
    {
        [FieldOrder(0)]
        public int Width { get; set; }

        [FieldOrder(1)]
        public int Height { get; set; }

        [FieldOrder(2)]
        public byte BitDepth { get; set; }

        [FieldOrder(3)]
        public PngColorType ColorType { get; set; }

        [FieldOrder(4)]
        public byte CompressionMethod { get; set; }

        [FieldOrder(5)]
        public byte FilterMethod { get; set; }
        
        [FieldOrder(6)]
        public byte InterlaceMethod { get; set; }
    }
}

using BinarySerialization;

namespace BinarySerializer.Editor.Test
{
    public class PngRgb
    {
        [FieldOrder(0)]
        public byte Red { get; set; }

        [FieldOrder(1)]
        public byte Green { get; set; }

        [FieldOrder(2)]
        public byte Blue { get; set; }
    }
}

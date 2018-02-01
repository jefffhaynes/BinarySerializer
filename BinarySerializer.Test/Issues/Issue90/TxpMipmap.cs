using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue90
{
    public class TxpMipmap : TxpBase
    {
        public enum TexFormat : uint
        {
            RGB = 1,
            RGBA = 2,
            DXT1 = 6,
            DXT3 = 7,
            DXT5 = 9,
            ATI2n = 11
        }

        public TxpMipmap() => Magic = "TXP"; //TXP2
        [FieldOrder(1)] public int Width = 512;
        [FieldOrder(2)] public int Height = 512;
        [FieldOrder(3)] public TexFormat Format = TexFormat.RGB;
        [FieldOrder(4)] public int Id;
        [FieldOrder(5)] public int ByteSize = 512 * 512;
        [FieldOrder(6), FieldCount("ByteSize")] public List<byte> Data;
        [Ignore] public int Size => 24 + Data.Count;

        public TxpMipmap(int width, int height, TexFormat format = TexFormat.RGB, int byteSize = 0)// : this()
        {
            Width = width;
            Height = height;
            Format = format;
            ByteSize = byteSize == 0 ? width * height : byteSize;
        }

   

        public override string ToString() => $"Mip #{Id}: {Width}x{Height} {Format} (BS:{ByteSize}, TS:n/a)";
    }
}
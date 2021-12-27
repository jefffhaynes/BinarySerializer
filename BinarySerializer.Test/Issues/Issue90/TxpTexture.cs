using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue90
{
    public class TxpTexture : TxpBase
    {
        public TxpTexture() => Magic = "TXP"; //TXP4

        [FieldOrder(1)]
        public int MipMapCount = 1;

        [FieldOrder(2)]
        public int Version = 0x1010101;

        [FieldOrder(3), FieldCount("MipMapCount")]
        public List<int> OffsetTable = new List<int>();

        [FieldOrder(4), FieldCount("MipMapCount")]
        public List<TxpMipmap> Mipmaps = new List<TxpMipmap>();

        //[Ignore] public int Size => 12 + OffsetTable.Count * 4 + Mipmaps.Sum(mip => mip?.Size ?? 0);

        //public void SetMipOffsets()
        //{
        //    OffsetTable = new List<int>(MipMapCount);
        //    var offset = 12 + MipMapCount * 4;
        //    for (var counter = 0; counter < Mipmaps.Count; ++counter)
        //    {
        //        offset += counter != 0 ? Mipmaps[counter - 1].Size : 0;
        //        OffsetTable.Add(offset);
        //    }
        //}
        

        public override string ToString() => $"TXP4: {MipMapCount} mips ( {Mipmaps[0].ToString().Substring(8)} )";
    }
}